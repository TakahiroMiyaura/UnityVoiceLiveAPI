// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Unity microphone audio capture component.
    ///     Captures microphone input and converts it to PCM16 format for Azure AI VoiceLive API.
    /// </summary>
    public class UnityAudioCapture : IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the UnityAudioCapture class.
        /// </summary>
        /// <param name="sampleRate">The sample rate for audio capture (default: 24000 Hz for Azure AI).</param>
        /// <param name="lengthSec">The length of the recording buffer in seconds (default: 10 seconds).</param>
        public UnityAudioCapture(int sampleRate = 24000, int lengthSec = 10)
        {
            SampleRate = sampleRate;
            this.lengthSec = lengthSec;
            audioBuffer = new float[sampleRate];
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        ///     Releases all resources used by the UnityAudioCapture.
        /// </summary>
        public void Dispose()
        {
            StopCapture();

            if (microphoneClip != null)
            {
                Object.Destroy(microphoneClip);
                microphoneClip = null;
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Fired when audio data is captured from the microphone.
        /// </summary>
        public event Action<byte[]> OnAudioDataCaptured;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Converts float audio samples to PCM16 format.
        /// </summary>
        /// <param name="floatSamples">The float audio samples (-1.0 to 1.0).</param>
        /// <param name="sampleCount">The number of samples to convert.</param>
        /// <returns>The PCM16 audio data as byte array.</returns>
        private byte[] ConvertToPCM16(float[] floatSamples, int sampleCount)
        {
            var pcm16Data = new byte[sampleCount * 2]; // 2 bytes per sample (16-bit)

            for (var i = 0; i < sampleCount; i++)
            {
                // Clamp float to [-1.0, 1.0]
                var sample = Mathf.Clamp(floatSamples[i], -1.0f, 1.0f);

                // Convert to 16-bit signed integer
                var pcm16Sample = (short)(sample * short.MaxValue);

                // Write as little-endian bytes
                pcm16Data[i * 2] = (byte)(pcm16Sample & 0xFF);
                pcm16Data[i * 2 + 1] = (byte)((pcm16Sample >> 8) & 0xFF);
            }

            return pcm16Data;
        }

        #endregion

        #region Private Fields

        private AudioClip microphoneClip;
        private string deviceName;
        private int lastSamplePosition;
        private float[] audioBuffer;
        private readonly int lengthSec;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether audio capture is active.
        /// </summary>
        public bool IsCapturing { get; private set; }

        /// <summary>
        ///     Gets the sample rate of the captured audio.
        /// </summary>
        public int SampleRate { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Starts capturing audio from the microphone.
        /// </summary>
        /// <param name="deviceName">The name of the microphone device (null for default device).</param>
        public void StartCapture(string deviceName = null)
        {
            if (IsCapturing)
            {
                Debug.LogWarning("Audio capture is already active");
                return;
            }

            this.deviceName = deviceName;

            // Get available microphone devices
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone devices found");
                return;
            }

            // Use default device if not specified
            if (string.IsNullOrEmpty(this.deviceName) && Microphone.devices.Length > 0)
            {
                this.deviceName = Microphone.devices[0];
            }

            // Start recording
            microphoneClip = Microphone.Start(this.deviceName, true, lengthSec, SampleRate);

            if (microphoneClip == null)
            {
                Debug.LogError("Failed to start microphone recording");
                return;
            }

            lastSamplePosition = 0;
            IsCapturing = true;
        }

        /// <summary>
        ///     Stops capturing audio from the microphone.
        /// </summary>
        public void StopCapture()
        {
            if (!IsCapturing)
            {
                return;
            }

            Microphone.End(deviceName);
            IsCapturing = false;
        }

        /// <summary>
        ///     Updates the audio capture and processes new audio data.
        ///     Must be called regularly (e.g., from Unity's Update() method).
        /// </summary>
        public void Update()
        {
            if (!IsCapturing || microphoneClip == null)
            {
                return;
            }

            var currentPosition = Microphone.GetPosition(deviceName);

            if (currentPosition < 0 || currentPosition == lastSamplePosition)
            {
                return;
            }

            // Handle wraparound
            int samplesAvailable;
            if (currentPosition < lastSamplePosition)
            {
                // Wraparound occurred
                samplesAvailable = microphoneClip.samples - lastSamplePosition + currentPosition;
            }
            else
            {
                samplesAvailable = currentPosition - lastSamplePosition;
            }

            if (samplesAvailable <= 0)
            {
                return;
            }

            // Resize buffer if necessary
            if (audioBuffer.Length < samplesAvailable)
            {
                audioBuffer = new float[samplesAvailable];
            }

            // Get audio data
            if (!microphoneClip.GetData(audioBuffer, lastSamplePosition))
            {
                Debug.LogWarning("Failed to get audio data from microphone clip");
                return;
            }

            // Convert to PCM16 and invoke event
            var pcm16Data = ConvertToPCM16(audioBuffer, samplesAvailable);
            OnAudioDataCaptured?.Invoke(pcm16Data);

            lastSamplePosition = currentPosition;
        }

        /// <summary>
        ///     Gets the list of available microphone devices.
        /// </summary>
        /// <returns>An array of microphone device names.</returns>
        public static string[] GetAvailableDevices()
        {
            return Microphone.devices;
        }

        #endregion
    }
}