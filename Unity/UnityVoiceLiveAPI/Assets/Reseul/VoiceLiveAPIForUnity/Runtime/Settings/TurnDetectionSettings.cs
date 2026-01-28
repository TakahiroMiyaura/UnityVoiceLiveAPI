// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for turn detection (Voice Activity Detection) in VoiceLive sessions.
    /// </summary>
    [Serializable]
    public class TurnDetectionSettings
    {
        #region Fields

        [Header("Turn Detection")]
        [Tooltip("Type of turn detection. Select from dropdown or choose 'Custom...' to enter a custom value.")]
        [VoiceLiveDropdown("TurnDetectionTypes")]
        [SerializeField]
        private string type = "server_vad";

        [Tooltip("Voice activity detection threshold (0.0-1.0)")]
        [Range(0f, 1f)]
        [SerializeField]
        private float threshold = 0.5f;

        [Tooltip("Duration of silence (in milliseconds) to detect end of speech")]
        [SerializeField]
        private int silenceDurationMs = 500;

        [Tooltip("Padding before the detected speech start (in milliseconds)")]
        [SerializeField]
        private int prefixPaddingMs = 300;

        [Header("Response Behavior")]
        [Tooltip("Automatically create response after turn detection")]
        [SerializeField]
        private bool createResponse = true;

        [Tooltip("Interrupt current response when new speech is detected")]
        [SerializeField]
        private bool interruptResponse = true;

        [Tooltip("Remove filler words from transcription")]
        [SerializeField]
        private bool removeFillerWords;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the turn detection type (API value string).
        /// </summary>
        public string Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        ///     Gets or sets the VAD threshold.
        /// </summary>
        public float Threshold
        {
            get => threshold;
            set => threshold = Mathf.Clamp01(value);
        }

        /// <summary>
        ///     Gets or sets the silence duration in milliseconds.
        /// </summary>
        public int SilenceDurationMs
        {
            get => silenceDurationMs;
            set => silenceDurationMs = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets the prefix padding in milliseconds.
        /// </summary>
        public int PrefixPaddingMs
        {
            get => prefixPaddingMs;
            set => prefixPaddingMs = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets whether to automatically create response.
        /// </summary>
        public bool CreateResponse
        {
            get => createResponse;
            set => createResponse = value;
        }

        /// <summary>
        ///     Gets or sets whether to interrupt response on new speech.
        /// </summary>
        public bool InterruptResponse
        {
            get => interruptResponse;
            set => interruptResponse = value;
        }

        /// <summary>
        ///     Gets or sets whether to remove filler words.
        /// </summary>
        public bool RemoveFillerWords
        {
            get => removeFillerWords;
            set => removeFillerWords = value;
        }

        #endregion
    }
}
