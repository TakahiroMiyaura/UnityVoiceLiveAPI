// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commands.Messages;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commons.Messages.Parts;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Avatar
{
    /// <summary>
    ///     Unity Avatar WebRTC client component.
    ///     Integrates with Unity WebRTC package for avatar video and audio streaming.
    /// </summary>
    /// <remarks>
    ///     This component requires the Unity WebRTC package (com.unity.webrtc) to be installed.
    ///     It provides a framework for integrating Azure AI Avatar streaming with Unity's WebRTC implementation.
    /// </remarks>
    public class UnityAvatarClient : MonoBehaviour
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the WebRTC connection is established.
        /// </summary>
        public bool IsConnected { get; private set; }

        #endregion

        #region Inspector Settings

#pragma warning disable CS0649 // Field is never assigned to - Unity serializes these via Inspector
        [Header("Display Settings")]
        [SerializeField]
        [Tooltip("RawImage component for displaying avatar video")]
        private RawImage videoDisplay;

        [SerializeField]
        [Tooltip("AudioSource for playing avatar audio")]
        private AudioSource audioSource;

        [Header("Debug")]
        [SerializeField]
        [Tooltip("Enable debug logging")]
        private bool enableDebugLogging = true;
#pragma warning restore CS0649

        #endregion

        #region Unity Events

        [Header("Events")]
        public UnityEvent OnConnectionEstablished;

        public UnityEvent OnConnectionClosed;
        public UnityEvent<string> OnError;
        public UnityEvent OnVideoFrameReceived;
        public UnityEvent OnAudioFrameReceived;

        #endregion

        #region Private Fields

        private RTCPeerConnection peerConnection;
        private Texture2D videoTexture;
        private TaskCompletionSource<string> offerCreatedTcs;
        private Coroutine webrtcUpdateCoroutine;
        private VideoStreamTrack currentVideoTrack;
        private AudioStreamTrack currentAudioTrack;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Note: Unity WebRTC 3.x+ auto-initializes, no explicit Initialize() needed

            // Validate components
            if (videoDisplay == null)
            {
                LogWarning("VideoDisplay RawImage not assigned");
            }

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            Log("UnityAvatarClient initialized");
        }

        private void OnDestroy()
        {
            Log("UnityAvatarClient OnDestroy called");

            // Cleanup
            DisconnectWebRTC();

            // Additional cleanup
            StopAllCoroutines();
        }

        private void OnApplicationQuit()
        {
            Log("Application quitting - forcing WebRTC cleanup");
            DisconnectWebRTC();
            StopAllCoroutines();
        }

        /// <summary>
        ///     Closes the WebRTC connection and stops all coroutines.
        /// </summary>
        public void Close()
        {
            DisconnectWebRTC();
            StopAllCoroutines();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Handles peer negotiation needed event (deprecated, use NegotiationProcess instead).
        /// </summary>
        /// <param name="pc">The RTCPeerConnection instance.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        private IEnumerator PeerNegotiationNeeded(RTCPeerConnection pc)
        {
            var op = pc.CreateOffer();
            yield return op;

            if (!op.IsError)
            {
                if (pc.SignalingState != RTCSignalingState.Stable)
                {
                    yield break;
                }

                var desc = op.Desc;
                yield return pc.SetLocalDescription(ref desc);
            }
        }

        /// <summary>
        ///     Initializes WebRTC connection with the provided ICE servers.
        /// </summary>
        /// <param name="iceServers">ICE servers configuration.</param>
        /// <returns>A session avatar connect message to send to the server.</returns>
        public async Task<SessionAvatarConnect> InitializeWebRTC(IceServers iceServers)
        {
            if (webrtcUpdateCoroutine == null)
            {
                webrtcUpdateCoroutine = StartCoroutine(WebRTC.Update());
            }

            Log("Initializing WebRTC");

            try
            {
                offerCreatedTcs = new TaskCompletionSource<string>();

                // RTCConfiguration
                var config = new RTCConfiguration
                {
                    iceServers = new[]
                    {
                        new RTCIceServer
                        {
                            urls = iceServers.Urls.ToArray(),
                            username = iceServers.UserName,
                            credential = iceServers.Credential,
                            credentialType = RTCIceCredentialType.Password
                        }
                    },
                    iceTransportPolicy = RTCIceTransportPolicy.All,
                    bundlePolicy = RTCBundlePolicy.BundlePolicyBalanced
                };

                peerConnection = new RTCPeerConnection(ref config);

                // Setup OnNegotiationNeeded handler
                peerConnection.OnNegotiationNeeded = () => { StartCoroutine(NegotiationProcess()); };

                // Setup other event handlers
                peerConnection.OnIceConnectionChange += HandleIceConnectionChange;
                peerConnection.OnTrack += HandleTrackAdded;
                peerConnection.OnIceCandidate += HandleIceCandidate;

                // Add transceivers (this will trigger OnNegotiationNeeded automatically)
                var videoTransceiver = peerConnection.AddTransceiver(TrackKind.Video);
                videoTransceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;

                var audioTransceiver = peerConnection.AddTransceiver(TrackKind.Audio);
                audioTransceiver.Direction = RTCRtpTransceiverDirection.RecvOnly;

                // Wait for Offer to be created in OnNegotiationNeeded
                var sdpJson = await offerCreatedTcs.Task;

                var avatarConnectMessage = new SessionAvatarConnect
                {
                    ClientSdp = Convert.ToBase64String(Encoding.UTF8.GetBytes(sdpJson))
                };

                return avatarConnectMessage;
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize WebRTC: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        ///     Sets the remote SDP answer from the server.
        /// </summary>
        /// <param name="sdpAnswer">The SDP answer in JSON format.</param>
        public IEnumerator SetRemoteAnswer(string sdpAnswer)
        {
            Log("Setting remote SDP answer");

            // Parse JSON
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(sdpAnswer);

            if (dict == null || !dict.ContainsKey("sdp"))
            {
                LogError("Failed to parse SDP JSON - missing 'sdp' key");
                yield return null;
            }

            var sdp = dict["sdp"].ToString()?.Replace("\\r\\n", "\r\n");

            // Create Answer description
            var answer = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = sdp
            };

            var setRemoteSdpOperation = peerConnection.SetRemoteDescription(ref answer);
            yield return new WaitUntil(() => setRemoteSdpOperation.IsDone);

            if (setRemoteSdpOperation.IsError)
            {
                LogError("Failed to set remote description");
                yield break;
            }

            // Manually get Track from Receiver and call handler
            var receivers = peerConnection.GetReceivers();

            foreach (var receiver in receivers)
            {
                if (receiver.Track != null)
                {
                    if (receiver.Track is VideoStreamTrack videoTrack)
                    {
                        HandleVideoTrack(videoTrack);
                    }
                    else if (receiver.Track is AudioStreamTrack audioTrack)
                    {
                        HandleAudioTrack(audioTrack);
                    }
                }
                else
                {
                    LogError("Receiver has null track");
                }
            }

            IsConnected = true;
            Log("Remote SDP answer set successfully");
            OnConnectionEstablished?.Invoke();
        }

        /// <summary>
        ///     Handles the WebRTC negotiation process including offer creation and ICE gathering.
        /// </summary>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        private IEnumerator NegotiationProcess()
        {
            Log("Creating WebRTC offer");

            // SetLocalDescription() without parameters auto-generates Offer
            var op = peerConnection.SetLocalDescription();
            yield return op;

            if (op.IsError)
            {
                LogError($"SetLocalDescription error: {op.Error.message}");
                offerCreatedTcs?.TrySetException(new Exception(op.Error.message));
                yield break;
            }

            // Wait for ICE gathering with timeout (10 seconds max)
            var timeout = 10f;
            var elapsed = 0f;
            while (peerConnection.GatheringState != RTCIceGatheringState.Complete && elapsed < timeout)
            {
                yield return new WaitForSeconds(0.5f);
                elapsed += 0.5f;
            }

            if (peerConnection.GatheringState != RTCIceGatheringState.Complete)
            {
                LogWarning($"ICE gathering timeout after {timeout}s, proceeding with current candidates");
            }
            else
            {
                Log("ICE gathering completed");
            }

            // Get the complete SDP
            var localSdp = peerConnection.LocalDescription.sdp;

            // Format SDP for Azure AI
            localSdp = localSdp.Replace("UDP/TLS/RTP/SAVP", "UDP/TLS/RTP/SAVPF");

            // Create JSON
            var escapedSdp = localSdp.Replace("\r", "\\r").Replace("\n", "\\n");
            var sdpJson = $"{{\"type\": \"offer\",\"sdp\": \"{escapedSdp}\"}}";

            // Complete the Task
            offerCreatedTcs?.TrySetResult(sdpJson);
        }

        /// <summary>
        ///     Starts video playback display.
        /// </summary>
        public void StartVideoPlayback()
        {
            if (!IsConnected)
            {
                LogWarning("Cannot start video playback: not connected");
                return;
            }

            Log("Video playback started");
        }

        /// <summary>
        ///     Stops video playback display.
        /// </summary>
        public void StopVideoPlayback()
        {
            if (videoDisplay != null && videoDisplay.texture != null)
            {
                videoDisplay.texture = null;
            }

            Log("Video playback stopped");
        }

        /// <summary>
        ///     Disconnects the WebRTC connection and cleans up resources.
        /// </summary>
        public void DisconnectWebRTC()
        {
            Log("Disconnecting WebRTC");

            try
            {
                // Clear VideoTrack event handler
                if (currentVideoTrack != null)
                {
                    currentVideoTrack.OnVideoReceived -= VideoFrameUpdate;
                    currentVideoTrack = null;
                }

                // AudioTrack cleanup
                if (currentAudioTrack != null)
                {
                    if (audioSource != null)
                    {
                        audioSource.Stop();
                        audioSource.clip = null;
                    }

                    currentAudioTrack = null;
                }

                // Unsubscribe event handlers
                if (peerConnection != null)
                {
                    peerConnection.OnNegotiationNeeded = null;
                    peerConnection.OnIceConnectionChange -= HandleIceConnectionChange;
                    peerConnection.OnTrack -= HandleTrackAdded;
                    peerConnection.OnIceCandidate -= HandleIceCandidate;
                }

                // PeerConnection cleanup
                if (peerConnection != null)
                {
                    peerConnection.Close();
                    peerConnection.Dispose();
                    peerConnection = null;
                }

                // Texture cleanup
                if (videoDisplay != null)
                {
                    var destroy = videoDisplay.texture;
                    videoDisplay.texture = null;
                    Destroy(destroy);
                }

                if (videoTexture != null)
                {
                    Destroy(videoTexture);
                    videoTexture = null;
                }

                // Stop WebRTC.Update() coroutine
                if (webrtcUpdateCoroutine != null)
                {
                    StopCoroutine(webrtcUpdateCoroutine);
                    webrtcUpdateCoroutine = null;
                }

                // Cancel TaskCompletionSource
                if (offerCreatedTcs != null && !offerCreatedTcs.Task.IsCompleted)
                {
                    offerCreatedTcs.TrySetCanceled();
                }

                offerCreatedTcs = null;

                IsConnected = false;
                Log("WebRTC disconnected");
                OnConnectionClosed?.Invoke();
            }
            catch (Exception ex)
            {
                LogError($"Error during disconnect: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods - WebRTC Event Handlers

        /// <summary>
        ///     Handles ICE connection state changes.
        /// </summary>
        /// <param name="state">The new ICE connection state.</param>
        private void HandleIceConnectionChange(RTCIceConnectionState state)
        {
            Log($"ICE Connection state changed: {state}");

            switch (state)
            {
                case RTCIceConnectionState.Connected:
                case RTCIceConnectionState.Completed:
                    IsConnected = true;
                    OnConnectionEstablished?.Invoke();
                    break;

                case RTCIceConnectionState.Disconnected:
                case RTCIceConnectionState.Failed:
                case RTCIceConnectionState.Closed:
                    IsConnected = false;
                    OnConnectionClosed?.Invoke();
                    break;
            }
        }

        /// <summary>
        ///     Handles when a new media track is added from the remote peer.
        /// </summary>
        /// <param name="trackEvent">The track event containing the new track.</param>
        private void HandleTrackAdded(RTCTrackEvent trackEvent)
        {
            Log($"Track added: {trackEvent.Track.Kind}");

            if (trackEvent.Track is VideoStreamTrack videoTrack)
            {
                HandleVideoTrack(videoTrack);
            }
            else if (trackEvent.Track is AudioStreamTrack audioTrack)
            {
                HandleAudioTrack(audioTrack);
            }
        }

        /// <summary>
        ///     Handles an incoming video track from the remote peer.
        /// </summary>
        /// <param name="videoTrack">The video stream track.</param>
        private void HandleVideoTrack(VideoStreamTrack videoTrack)
        {
            Log("Video track received");
            currentVideoTrack = videoTrack;

            // Setup video frame callback
            videoTrack.OnVideoReceived += VideoFrameUpdate;
        }

        /// <summary>
        ///     Updates the video display with the received texture.
        /// </summary>
        /// <param name="texture">The received video texture.</param>
        private void VideoFrameUpdate(Texture texture)
        {
            // Update every frame
            if (videoDisplay != null && texture != null)
            {
                videoDisplay.texture = texture;
            }

            OnVideoFrameReceived?.Invoke();
        }

        /// <summary>
        ///     Handles an incoming audio track from the remote peer.
        /// </summary>
        /// <param name="audioTrack">The audio stream track.</param>
        private void HandleAudioTrack(AudioStreamTrack audioTrack)
        {
            Log("Audio track received");
            currentAudioTrack = audioTrack;

            // Setup audio playback
            if (audioSource != null)
            {
                audioSource.SetTrack(audioTrack);
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                LogError("AudioSource is null");
            }

            OnAudioFrameReceived?.Invoke();
        }

        /// <summary>
        ///     Handles ICE candidate events (candidates are gathered automatically).
        /// </summary>
        /// <param name="candidate">The ICE candidate.</param>
        private void HandleIceCandidate(RTCIceCandidate candidate)
        {
            // ICE candidates are gathered automatically
        }

        #endregion

        #region Private Methods - Logging

        /// <summary>
        ///     Logs a debug message to the Unity Console if debug logging is enabled.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void Log(string message)
        {
            if (enableDebugLogging)
            {
                Debug.Log($"[UnityAvatarClient] {message}");
            }
        }

        /// <summary>
        ///     Logs a warning message to the Unity Console if debug logging is enabled.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        private void LogWarning(string message)
        {
            if (enableDebugLogging)
            {
                Debug.LogWarning($"[UnityAvatarClient] {message}");
            }
        }

        /// <summary>
        ///     Logs an error message to the Unity Console.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        private void LogError(string message)
        {
            Debug.LogError($"[UnityAvatarClient] {message}");
        }

        #endregion

        #region Helper Classes

        /// <summary>
        ///     ICE servers configuration class.
        /// </summary>
        [Serializable]
        public class IceServersConfig
        {
            public IceServer[] iceServers { get; set; }
        }

        /// <summary>
        ///     ICE server class.
        /// </summary>
        [Serializable]
        public class IceServer
        {
            public string[] urls { get; set; }
            public string username { get; set; }
            public string credential { get; set; }
        }

        #endregion
    }
}