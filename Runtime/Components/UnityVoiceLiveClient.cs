// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Core;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commands.Messages;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Logs;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Models;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Avatar;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings.Converters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components
{
    /// <summary>
    ///     Unity MonoBehaviour wrapper for VoiceLive sessions.
    ///     Provides Unity integration for Microsoft Foundry VoiceLive API connections.
    /// </summary>
    /// <remarks>
    ///     This component supports both AI Agent mode and AI Model mode via ScriptableObject configuration.
    ///     Configure connection settings via <see cref="FoundryConnectionSettings"/> and
    ///     session settings via <see cref="VoiceLiveSessionSettings"/>.
    /// </remarks>
    public class UnityVoiceLiveClient : MonoBehaviour
    {
        #region Inspector Settings

#pragma warning disable CS0649 // Field is never assigned to - Unity serializes these via Inspector
        [Header("Connection Settings")]
        [Tooltip("Foundry connection settings ScriptableObject containing endpoint, authentication, and agent configuration.")]
        [SerializeField]
        private FoundryConnectionSettings connectionSettings;

        [Header("Session Settings")]
        [Tooltip("Session settings ScriptableObject containing voice, audio, turn detection, and avatar configuration.")]
        [SerializeField]
        private VoiceLiveSessionSettings sessionSettings;

        [Header("Audio Settings")]
        [Tooltip("AudioSource for playing AI responses. If not assigned, one will be created automatically.")]
        [SerializeField]
        private AudioSource audioSource;

        [Tooltip("Automatically start recording on session start.")]
        [SerializeField]
        private bool autoStartRecording;

        [Header("Avatar")]
        [Tooltip("Avatar client for WebRTC video streaming. If assigned, avatar mode is enabled.")]
        [SerializeField]
        private UnityAvatarClient avatarClient;

        [Header("Logging")]
        [Tooltip("Enable debug logging to Unity Console.")]
        [SerializeField]
        private LogLevel logLevel = LogLevel.Error;
#pragma warning restore CS0649

        #endregion

        #region Unity Events

        [Header("Events")]
        [Tooltip("Invoked when a user transcript is received.")]
        public UnityEvent<string> OnTranscriptReceived;

        [Tooltip("Invoked when an AI response is completed.")]
        public UnityEvent<string> OnResponseOutputItemDoneReceived;

        [Tooltip("Invoked when an error occurs.")]
        public UnityEvent<string> OnErrorOccurred;

        [Tooltip("Invoked when the session starts.")]
        public UnityEvent OnSessionStarted;

        [Tooltip("Invoked when the session ends.")]
        public UnityEvent OnSessionEnded;

        [Tooltip("Invoked when connected to the server.")]
        public UnityEvent OnConnected;

        [Tooltip("Invoked when disconnected from the server.")]
        public UnityEvent OnDisconnected;

        #endregion

        #region Private Fields

        private VoiceLiveClient voiceLiveClient;
        private VoiceLiveSession session;
        private ServerMessageHandlerManager messageHandler;
        private AvatarMessageHandlerManager avatarManager;
        private UnityAudioCapture audioCapture;
        private UnityAudioPlayback audioPlayback;
        private bool avatarInitialized;
        private bool avatarInitializationPending;
        private SessionInfo pendingAvatarSessionInfo;
        private bool remoteAnswerPending;
        private string pendingRemoteAnswerSdp;
        private readonly System.Collections.Concurrent.ConcurrentQueue<string> pendingErrorMessages = new();
        private readonly System.Collections.Concurrent.ConcurrentQueue<string> pendingTranscripts = new();
        private readonly System.Collections.Concurrent.ConcurrentQueue<string> pendingResponses = new();
        private volatile bool pendingSessionStarted;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the client is connected to the server.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether audio recording is active.
        /// </summary>
        public bool IsRecording { get; private set; }

        /// <summary>
        ///     Gets the underlying session instance.
        /// </summary>
        public VoiceLiveSession Session => session;

        /// <summary>
        ///     Gets or sets the connection settings.
        /// </summary>
        public FoundryConnectionSettings ConnectionSettings
        {
            get => connectionSettings;
            set => connectionSettings = value;
        }

        /// <summary>
        ///     Gets or sets the session settings.
        /// </summary>
        public VoiceLiveSessionSettings SessionSettings
        {
            get => sessionSettings;
            set => sessionSettings = value;
        }

        /// <summary>
        ///     Gets the avatar client.
        /// </summary>
        public UnityAvatarClient AvatarClient => avatarClient;

        /// <summary>
        ///     Gets a value indicating whether avatar mode is enabled.
        /// </summary>
        public bool IsAvatarEnabled => avatarClient != null && sessionSettings?.HasAvatar == true;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Validate settings
            if (connectionSettings == null)
            {
                LogWarning("ConnectionSettings is not assigned. Please assign a FoundryConnectionSettings asset.");
            }

            if (sessionSettings == null)
            {
                LogWarning("SessionSettings is not assigned. Please assign a VoiceLiveSessionSettings asset.");
            }

            // Validate AudioSource
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            // Get sample rate from session settings or use default
            var sampleRate = sessionSettings?.AudioProcessing?.SampleRate ?? 24000;

            // Initialize audio components
            audioCapture = new UnityAudioCapture(sampleRate);
            audioPlayback = new UnityAudioPlayback(audioSource);

            // Initialize message handlers (will be added to session after connection)
            messageHandler = new ServerMessageHandlerManager();
            avatarManager = new AvatarMessageHandlerManager();
            avatarManager.OnSessionAvatarConnecting += AvatarManager_OnSessionAvatarConnecting;

            // Setup event handlers
            SetupMessageHandlers();
            audioCapture.OnAudioDataCaptured += HandleAudioDataCaptured;

            Log("UnityVoiceLiveClient initialized");
        }

        private void Update()
        {
            // Process pending avatar initialization on main thread
            ProcessPendingAvatarInitialization();

            // Process pending remote SDP answer on main thread
            ProcessPendingRemoteAnswer();

            // Process pending UnityEvent invocations on main thread
            ProcessPendingSessionStarted();
            ProcessPendingTranscripts();
            ProcessPendingResponses();
            ProcessPendingErrorMessages();

            // Update audio capture
            if (IsRecording)
            {
                audioCapture?.Update();
            }

            // Update audio playback
            audioPlayback?.Update();
        }

        /// <summary>
        ///     Processes pending session started events and fires OnSessionStarted on the main thread.
        /// </summary>
        private void ProcessPendingSessionStarted()
        {
            if (pendingSessionStarted)
            {
                pendingSessionStarted = false;
                OnSessionStarted?.Invoke();
            }
        }

        /// <summary>
        ///     Processes pending transcripts and fires OnTranscriptReceived events on the main thread.
        /// </summary>
        private void ProcessPendingTranscripts()
        {
            while (pendingTranscripts.TryDequeue(out var transcript))
            {
                OnTranscriptReceived?.Invoke(transcript);
            }
        }

        /// <summary>
        ///     Processes pending responses and fires OnResponseOutputItemDoneReceived events on the main thread.
        /// </summary>
        private void ProcessPendingResponses()
        {
            while (pendingResponses.TryDequeue(out var response))
            {
                OnResponseOutputItemDoneReceived?.Invoke(response);
            }
        }

        /// <summary>
        ///     Processes pending error messages and fires OnErrorOccurred events on the main thread.
        /// </summary>
        private void ProcessPendingErrorMessages()
        {
            while (pendingErrorMessages.TryDequeue(out var errorMessage))
            {
                OnErrorOccurred?.Invoke(errorMessage);
            }
        }

        private void OnDestroy()
        {
            // Cleanup - use synchronous cleanup to avoid deadlock in Unity
            Log("OnDestroy called - cleaning up...");

            try
            {
                StopRecording();

                // Don't use .Wait() - it causes deadlock in Unity
                // Instead, dispose synchronously
                if (session != null)
                {
                    session.Dispose();
                    session = null;
                }

                IsConnected = false;
                avatarInitialized = false;
            }
            catch (Exception ex)
            {
                LogError($"Cleanup error: {ex.Message}");
            }

            audioCapture?.Dispose();
            audioPlayback?.Dispose();

            Log("Cleanup completed");
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Connects to the Azure AI VoiceLive API.
        /// </summary>
        /// <returns>True if connection was successful, false otherwise.</returns>
        public async Task<bool> Connect()
        {
            if (IsConnected)
            {
                LogWarning("Already connected");
                return true;
            }

            // Validate settings
            if (connectionSettings == null)
            {
                LogError("ConnectionSettings is not assigned. Cannot connect.");
                OnErrorOccurred?.Invoke("ConnectionSettings is not assigned.");
                return false;
            }

            if (!connectionSettings.IsValid)
            {
                LogError("ConnectionSettings is invalid. Please check endpoint, accessToken, projectName, and agentId.");
                OnErrorOccurred?.Invoke("ConnectionSettings is invalid.");
                return false;
            }

            if (sessionSettings == null)
            {
                LogError("SessionSettings is not assigned. Cannot connect.");
                OnErrorOccurred?.Invoke("SessionSettings is not assigned.");
                return false;
            }

            try
            {
                Log("Connecting to Azure AI VoiceLive API...");
                Log($"  Endpoint: {connectionSettings.Endpoint}");
                Log($"  Project: {connectionSettings.ProjectName}");
                Log($"  Agent: {connectionSettings.AgentId}");
                Log($"  Auth Type: {connectionSettings.AuthenticationType}");
                Log($"  Connection Mode: {sessionSettings.ConnectionMode}");
                ILoggerFactory loggerFactory = LoggerFactory.Create(configure =>
                {
                    configure.SetMinimumLevel(logLevel);
                    configure.AddProvider(new UnityLoggerProvider(logLevel));
                });
                LoggerFactoryManager.Set(loggerFactory);
                // Initialize VoiceLive client
                var clientOptions = new VoiceLiveClientOptions { ApiVersion = connectionSettings.ApiVersion };
                voiceLiveClient = new VoiceLiveClient(
                    connectionSettings.Endpoint,
                    connectionSettings.AccessToken,
                    connectionSettings.AuthenticationType,
                    clientOptions);
                voiceLiveClient.AgentAccessToken = connectionSettings.EffectiveAgentAccessToken;

                // Convert session settings to session options using the converter
                var sessionOptions = sessionSettings.ToSessionOptions();

                // Prepare message handlers to register BEFORE connecting
                // This ensures session.created and session.updated events are captured
                var handlers = IsAvatarEnabled
                    ? new Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commons.MessageHandlerManagerBase[]
                        { messageHandler, avatarManager }
                    : new Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commons.MessageHandlerManagerBase[]
                        { messageHandler };

                // Start session based on connection mode
                if (sessionSettings.IsAgentMode)
                {
                    Log("Starting Agent session...");
                    session = await voiceLiveClient.StartAgentSessionAsync(
                        connectionSettings.ProjectName,
                        connectionSettings.AgentId,
                        sessionOptions,
                        handlers);
                }
                else
                {
                    Log("Starting Model session...");
                    session = await voiceLiveClient.StartSessionAsync(sessionOptions);

                    // Add message handlers after session creation for Model mode
                    foreach (var handler in handlers)
                    {
                        session.AddMessageHandlerManager(handler);
                    }
                }

                if (IsAvatarEnabled)
                {
                    Log("Avatar mode enabled");
                }

                IsConnected = true;
                Log("Connected successfully");
                OnConnected?.Invoke();

                if (autoStartRecording)
                {
                    StartRecording();
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError($"Connection failed: {ex.Message}");
                OnErrorOccurred?.Invoke($"Connection failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Disconnects from the Azure AI VoiceLive API.
        /// </summary>
        public async Task Disconnect()
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                StopRecording();

                if (session != null)
                {
                    await session.DisposeAsync();
                    session = null;
                }

                IsConnected = false;
                OnDisconnected?.Invoke();
                Log("Disconnected");
            }
            catch (Exception ex)
            {
                LogError($"Disconnect error: {ex.Message}");
            }
        }

        /// <summary>
        ///     Starts recording audio from the microphone.
        /// </summary>
        public void StartRecording()
        {
            if (!IsConnected)
            {
                LogWarning("Cannot start recording: not connected");
                return;
            }

            if (IsRecording)
            {
                LogWarning("Already recording");
                return;
            }

            audioCapture.StartCapture();
            IsRecording = true;
            Log("Recording started");
        }

        /// <summary>
        ///     Stops recording audio from the microphone.
        /// </summary>
        public void StopRecording()
        {
            if (!IsRecording)
            {
                return;
            }

            audioCapture.StopCapture();
            IsRecording = false;
            Log("Recording stopped");
        }

        /// <summary>
        ///     Clears the audio playback queue.
        /// </summary>
        public void ClearAudioQueue()
        {
            audioPlayback?.ClearQueue();
            session?.ClearAudioQueue();
            Log("Audio queue cleared");
        }

        /// <summary>
        ///     Sends a text message to the AI.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async Task SendTextMessage(string message)
        {
            if (!IsConnected || session == null)
            {
                LogWarning("Cannot send message: not connected");
                return;
            }

            try
            {
                await session.SendUserMessageAsync(message);
                Log($"Text message sent: {message}");

                // Trigger AI response generation (required for text input, unlike voice which auto-triggers via turn detection)
                await session.CreateResponseAsync();
                Log("Response generation requested");
            }
            catch (Exception ex)
            {
                LogError($"Failed to send text message: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods - Audio

        /// <summary>
        ///     Handles captured audio data from the microphone.
        ///     Sends PCM16 audio data to the VoiceLive session asynchronously.
        /// </summary>
        /// <param name="pcm16Data">The captured PCM16 audio data.</param>
        private void HandleAudioDataCaptured(byte[] pcm16Data)
        {
            if (!IsConnected || !IsRecording || session == null)
            {
                return;
            }

            try
            {
                session.SendInputAudioAsync(pcm16Data).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        var errorMessage = $"Failed to send audio data: {task.Exception?.Message}";
                        LogError(errorMessage);
                        // Queue error for main thread processing
                        pendingErrorMessages.Enqueue(errorMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to process captured audio: {ex.Message}";
                LogError(errorMessage);
                OnErrorOccurred?.Invoke(errorMessage);
            }
        }

        #endregion

        #region Private Methods - Avatar

        /// <summary>
        ///     Handles the session.avatar.connecting event.
        ///     Queues the remote SDP for processing on the main thread since StartCoroutine cannot be called from background threads.
        /// </summary>
        private void AvatarManager_OnSessionAvatarConnecting(AvatarConnecting obj)
        {
            Log("session.avatar.connecting received");
            pendingRemoteAnswerSdp = obj.ServerSdp;
            remoteAnswerPending = true;
        }

        /// <summary>
        ///     Processes pending remote answer SDP on the main thread.
        /// </summary>
        private void ProcessPendingRemoteAnswer()
        {
            if (!remoteAnswerPending || string.IsNullOrEmpty(pendingRemoteAnswerSdp) || avatarClient == null)
            {
                return;
            }

            remoteAnswerPending = false;
            var sdp = pendingRemoteAnswerSdp;
            pendingRemoteAnswerSdp = null;

            Log("Setting remote SDP answer");
            StartCoroutine(avatarClient.SetRemoteAnswer(sdp));
        }

        /// <summary>
        ///     Queues avatar initialization to run on the main thread.
        ///     This method is safe to call from background threads (WebSocket handlers).
        ///     Note: session may be null when this is called from session.updated event
        ///     (event fires during await, before session variable is assigned).
        /// </summary>
        private void TryInitializeAvatar(SessionInfo sessionInfo)
        {
            // Skip if already initialized or pending, or no AvatarClient
            // Note: Do NOT check session == null here - the event fires before await completes
            if (avatarInitialized || avatarInitializationPending || avatarClient == null)
            {
                return;
            }

            // Check if Avatar and IceServers are available
            if (sessionInfo?.Avatar?.IceServers == null || sessionInfo.Avatar.IceServers.Length == 0)
            {
                return;
            }

            // Queue initialization for main thread processing
            pendingAvatarSessionInfo = sessionInfo;
            avatarInitializationPending = true;
        }

        /// <summary>
        ///     Processes pending avatar initialization on the main thread.
        ///     Called from Update() to ensure Unity API compatibility.
        /// </summary>
        private async void ProcessPendingAvatarInitialization()
        {
            if (!avatarInitializationPending || pendingAvatarSessionInfo == null)
            {
                return;
            }

            // Wait for session to be available (it's assigned after await StartAgentSessionAsync completes)
            if (session == null)
            {
                // Keep pending flag and retry next frame
                return;
            }

            avatarInitializationPending = false;
            var sessionInfo = pendingAvatarSessionInfo;
            pendingAvatarSessionInfo = null;

            try
            {
                avatarInitialized = true; // Set flag before async operation to prevent race conditions
                Log("Initializing WebRTC");
                var data = await avatarClient.InitializeWebRTC(sessionInfo.Avatar.IceServers[0]);
                await data.SendAsync(session);
                Log("Avatar connection request sent");
            }
            catch (Exception ex)
            {
                avatarInitialized = false; // Reset flag on failure to allow retry
                LogError($"Avatar initialization failed: {ex.Message}");
                LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        #endregion

        #region Private Methods - Setup

        /// <summary>
        ///     Configures message handlers for the VoiceLive session.
        ///     Sets up handlers for audio, transcription, session events, and errors.
        /// </summary>
        private void SetupMessageHandlers()
        {
            // Get sample rate from session settings
            var sampleRate = sessionSettings?.AudioProcessing?.SampleRate ?? 24000;

            // Audio delta handler
            messageHandler.OnAudioDeltaReceived += delta =>
            {
                if (!string.IsNullOrEmpty(delta.Delta))
                {
                    var audioData = Convert.FromBase64String(delta.Delta);
                    // EnqueuePCM16Data is thread-safe; playback auto-starts in Update() on main thread
                    audioPlayback.EnqueuePCM16Data(audioData, sampleRate);
                }
            };

            // Transcription handler
            messageHandler.OnTranscriptionReceived += transcription =>
            {
                if (!string.IsNullOrEmpty(transcription.Transcript))
                {
                    Log($"Transcript: {transcription.Transcript}");

                    // Queue for main thread processing (UnityEvents must be invoked on main thread)
                    pendingTranscripts.Enqueue(transcription.Transcript);
                }
            };

            messageHandler.OnResponseOutputItemDoneReceived += outputItem =>
            {
                Log("Response output item done received");
                var msg = outputItem.Item.Content?.Select(x => x.Transcript).Aggregate((a, b) => a + "\n" + b);
                if (!string.IsNullOrEmpty(msg))
                {
                    // Queue for main thread processing (UnityEvents must be invoked on main thread)
                    pendingResponses.Enqueue($"[{outputItem.Item.Role}]:\n{msg}\n");
                }
            };

            // Session created handler - just log, Avatar config was already sent in initial session.update
            messageHandler.OnSessionCreatedReceived += sessionCreated =>
            {
                Log("Session created");
            };

            // Session update handler
            messageHandler.OnSessionUpdateReceived += sessionInfo =>
            {
                Log("Session updated");
                // Queue for main thread processing (UnityEvents must be invoked on main thread)
                pendingSessionStarted = true;
                // Try avatar initialization from session.updated as well
                TryInitializeAvatar(sessionInfo);
            };

            // Error handler
            messageHandler.OnErrorReceived += error =>
            {
                LogError($"Server error: {error?.Code} - {error?.Message}");
            };
        }

        #endregion

        #region Private Methods - Logging

        /// <summary>
        ///     Logs a debug message to the Unity Console if the log level permits.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void Log(string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                case LogLevel.Trace:
                    Debug.Log($"[UnityVoiceLiveClient] {message}");
                    return;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Logs a warning message to the Unity Console if the log level permits.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        private void LogWarning(string message)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    Debug.LogWarning($"[UnityVoiceLiveClient] {message}");
                    return;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Logs an error message to the Unity Console if the log level permits.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        private void LogError(string message)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                case LogLevel.Warning:
                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    Debug.LogError($"[UnityVoiceLiveClient] {message}");
                    return;
                default:
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Unity-compatible ILoggerProvider implementation for Microsoft.Extensions.Logging.
    ///     Creates <see cref="UnityLogger"/> instances that output to Unity's Debug.Log.
    /// </summary>
    internal class UnityLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel minimumLevel;
        private UnityLogger logger;

        /// <summary>
        ///     Initializes a new instance of the UnityLoggerProvider class.
        /// </summary>
        /// <param name="minimumLevel">The minimum log level to output.</param>
        public UnityLoggerProvider(LogLevel minimumLevel = LogLevel.Error)
        {
            this.minimumLevel = minimumLevel;
        }

        /// <summary>
        ///     Creates a new logger instance for the specified category.
        /// </summary>
        /// <param name="categoryName">The category name for the logger.</param>
        /// <returns>A Unity-compatible logger instance.</returns>
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            if (logger == null)
            {
                logger = new UnityLogger(minimumLevel);
            }
            return logger;
        }

        /// <summary>
        ///     Disposes the logger provider and releases resources.
        /// </summary>
        public void Dispose()
        {
            logger = null;
        }
    }

    /// <summary>
    ///     Unity-compatible ILogger implementation that outputs to Unity's Debug.Log.
    /// </summary>
    internal class UnityLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly LogLevel minimumLevel;

        /// <summary>
        ///     Initializes a new instance of the UnityLogger class.
        /// </summary>
        /// <param name="minimumLevel">The minimum log level to output.</param>
        public UnityLogger(LogLevel minimumLevel = LogLevel.Error)
        {
            this.minimumLevel = minimumLevel;
        }

        /// <summary>
        ///     Logs a message to Unity's Debug.Log.
        /// </summary>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Debug.LogError(formatter.Invoke(state, exception));
                    return;
                case LogLevel.Warning:
                    Debug.LogWarning(formatter.Invoke(state, exception));
                    return;
                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    Debug.Log(formatter.Invoke(state, exception));
                    return;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Returns whether logging is enabled for the specified log level.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= minimumLevel;
        }

        /// <summary>
        ///     Begins a logical operation scope.
        /// </summary>
        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }
    }
}
