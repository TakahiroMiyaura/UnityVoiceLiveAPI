// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for audio processing in VoiceLive sessions.
    /// </summary>
    [Serializable]
    public class AudioProcessingSettings
    {
        #region Fields

        [Header("Audio Format")]
        [Tooltip("Audio sample rate in Hz")]
        [SerializeField]
        private int sampleRate = 24000;

        [Tooltip("Input audio format")]
        [SerializeField]
        private string inputAudioFormat = "pcm16";

        [Tooltip("Output audio format")]
        [SerializeField]
        private string outputAudioFormat = "pcm16";

        [Header("Noise Reduction")]
        [Tooltip("Type of noise reduction to apply. Select from dropdown or choose 'Custom...' to enter a custom value.")]
        [VoiceLiveDropdown("NoiseReductionTypes")]
        [SerializeField]
        private string noiseReduction = "azure_deep_noise_suppression";

        [Header("Echo Cancellation")]
        [Tooltip("Enable echo cancellation")]
        [SerializeField]
        private bool enableEchoCancellation;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the audio sample rate in Hz.
        /// </summary>
        public int SampleRate
        {
            get => sampleRate;
            set => sampleRate = value;
        }

        /// <summary>
        ///     Gets or sets the input audio format.
        /// </summary>
        public string InputAudioFormat
        {
            get => inputAudioFormat;
            set => inputAudioFormat = value;
        }

        /// <summary>
        ///     Gets or sets the output audio format.
        /// </summary>
        public string OutputAudioFormat
        {
            get => outputAudioFormat;
            set => outputAudioFormat = value;
        }

        /// <summary>
        ///     Gets or sets the noise reduction type (API value string).
        /// </summary>
        public string NoiseReduction
        {
            get => noiseReduction;
            set => noiseReduction = value;
        }

        /// <summary>
        ///     Gets or sets whether echo cancellation is enabled.
        /// </summary>
        public bool EnableEchoCancellation
        {
            get => enableEchoCancellation;
            set => enableEchoCancellation = value;
        }

        #endregion
    }
}
