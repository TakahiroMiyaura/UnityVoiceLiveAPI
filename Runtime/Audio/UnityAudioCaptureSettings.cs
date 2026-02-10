// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Audio capture settings for Unity's built-in Microphone API.
    ///     Create an instance via Assets > Create > VoiceLive API > Audio Capture > Unity Microphone.
    /// </summary>
    [CreateAssetMenu(
        fileName = "UnityAudioCaptureSettings",
        menuName = "VoiceLive API/Audio Capture/Unity Microphone",
        order = 100)]
    public class UnityAudioCaptureSettings : AudioCaptureSettings
    {
        #region Inspector Fields

        [Header("Device Settings")]
        [Tooltip("The microphone device name. Leave empty to use the default device. " +
                 "Use UnityAudioCapture.GetAvailableDevices() to get available device names.")]
        [SerializeField]
        private string deviceName = "";

        [Header("Buffer Settings")]
        [Tooltip("The length of the recording buffer in seconds.")]
        [SerializeField]
        [Range(1, 60)]
        private int bufferLengthSeconds = 10;

        [Header("Converter Settings")]
        [Tooltip("The audio converter type to use.")]
        [SerializeField]
        private AudioConverterType converterType = AudioConverterType.PCM16;

        #endregion

        #region Enums

        /// <summary>
        ///     Available audio converter types.
        /// </summary>
        public enum AudioConverterType
        {
            /// <summary>
            ///     PCM 16-bit signed integer format (default for Azure AI VoiceLive API).
            /// </summary>
            PCM16 = 0
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the microphone device name.
        ///     Empty string means the default device will be used.
        /// </summary>
        public string DeviceName
        {
            get => deviceName;
            set => deviceName = value;
        }

        /// <summary>
        ///     Gets or sets the recording buffer length in seconds.
        /// </summary>
        public int BufferLengthSeconds
        {
            get => bufferLengthSeconds;
            set => bufferLengthSeconds = Mathf.Clamp(value, 1, 60);
        }

        /// <summary>
        ///     Gets or sets the audio converter type.
        /// </summary>
        public AudioConverterType ConverterType
        {
            get => converterType;
            set => converterType = value;
        }

        /// <inheritdoc />
        public override string Description =>
            string.IsNullOrEmpty(deviceName)
                ? "Unity Microphone (Default Device)"
                : $"Unity Microphone ({deviceName})";

        #endregion

        #region AudioCaptureSettings Implementation

        /// <inheritdoc />
        public override IAudioCapture CreateAudioCapture(int sampleRate)
        {
            var converter = CreateConverter();
            var capture = new UnityAudioCapture(sampleRate, bufferLengthSeconds, converter);

            Debug.Log($"[UnityAudioCaptureSettings] Created audio capture: {Description}, SampleRate={sampleRate}");

            return new UnityAudioCaptureWrapper(capture, deviceName);
        }

        /// <summary>
        ///     Creates an audio converter based on the converter type setting.
        /// </summary>
        private IAudioConverter CreateConverter()
        {
            return converterType switch
            {
                AudioConverterType.PCM16 => new PCM16Converter(),
                _ => new PCM16Converter()
            };
        }

        #endregion
    }

    /// <summary>
    ///     Wrapper class that passes device name to StartCapture.
    /// </summary>
    internal class UnityAudioCaptureWrapper : IAudioCapture
    {
        private readonly UnityAudioCapture innerCapture;
        private readonly string deviceName;

        public UnityAudioCaptureWrapper(UnityAudioCapture capture, string deviceName)
        {
            innerCapture = capture;
            this.deviceName = deviceName;
        }

        public event System.Action<byte[]> OnAudioDataCaptured
        {
            add => innerCapture.OnAudioDataCaptured += value;
            remove => innerCapture.OnAudioDataCaptured -= value;
        }

        public bool IsCapturing => innerCapture.IsCapturing;
        public int SampleRate => innerCapture.SampleRate;

        public void StartCapture(string overrideDeviceName = null)
        {
            // Use the configured device name if no override is provided
            var effectiveDeviceName = string.IsNullOrEmpty(overrideDeviceName) ? deviceName : overrideDeviceName;
            innerCapture.StartCapture(string.IsNullOrEmpty(effectiveDeviceName) ? null : effectiveDeviceName);
        }

        public void StopCapture() => innerCapture.StopCapture();
        public void Update() => innerCapture.Update();
        public void Dispose() => innerCapture.Dispose();
    }
}
