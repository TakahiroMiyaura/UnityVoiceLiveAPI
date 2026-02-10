// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;
using Object = UnityEngine.Object;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Unity microphone audio capture implementation.
    ///     Captures microphone input using Unity's Microphone API.
    /// </summary>
    public class UnityAudioCapture : AudioCaptureBase
    {
        #region Private Fields

        private AudioClip microphoneClip;
        private string deviceName;
        private int lastSamplePosition;
        private readonly int lengthSec;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the UnityAudioCapture class.
        /// </summary>
        /// <param name="sampleRate">The sample rate for audio capture (default: 24000 Hz for Azure AI).</param>
        /// <param name="lengthSec">The length of the recording buffer in seconds (default: 10 seconds).</param>
        /// <param name="converter">The audio converter to use (default: PCM16Converter).</param>
        public UnityAudioCapture(int sampleRate = 24000, int lengthSec = 10, IAudioConverter converter = null)
            : base(sampleRate, converter)
        {
            this.lengthSec = lengthSec;
        }

        #endregion

        #region AudioCaptureBase Overrides

        /// <inheritdoc />
        public override void StartCapture(string deviceName = null)
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

            Debug.Log($"Started audio capture on device: {this.deviceName}");
        }

        /// <inheritdoc />
        public override void StopCapture()
        {
            if (!IsCapturing)
            {
                return;
            }

            Microphone.End(deviceName);
            IsCapturing = false;

            Debug.Log($"Stopped audio capture on device: {deviceName}");
        }

        /// <inheritdoc />
        public override void Update()
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

            // Ensure buffer has sufficient capacity
            EnsureBufferCapacity(samplesAvailable);

            // Get audio data
            if (!microphoneClip.GetData(audioBuffer, lastSamplePosition))
            {
                Debug.LogWarning("Failed to get audio data from microphone clip");
                return;
            }

            // Convert and raise event
            ProcessAndRaiseAudioData(audioBuffer, samplesAvailable);

            lastSamplePosition = currentPosition;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && microphoneClip != null)
            {
                Object.Destroy(microphoneClip);
                microphoneClip = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the list of available microphone devices.
        /// </summary>
        /// <returns>An array of microphone device names.</returns>
        public static string[] GetAvailableDevices()
        {
            return Microphone.devices;
        }

        /// <summary>
        ///     Gets the currently selected device name.
        /// </summary>
        public string CurrentDeviceName => deviceName;

        #endregion
    }
}
