// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for voice configuration in VoiceLive sessions.
    /// </summary>
    [Serializable]
    public class VoiceSettings
    {
        #region Fields

        // Note: Headers are drawn by VoiceSettingsDrawer to avoid overlap issues
        [Tooltip("Language/locale for voice selection. This filters the available voices.")]
        [SerializeField]
        private string language = "ja-JP";

        [Tooltip("Azure Neural Voice name. Filtered by the selected language.")]
        [SerializeField]
        private string voiceName = "ja-JP-Nanami:DragonHDLatestNeural";

        [Tooltip("Voice type. Select from dropdown or choose 'Custom...' to enter a custom value.")]
        [SerializeField]
        private string voiceType = "azure-standard";

        [Tooltip("Temperature for response generation (0.0-2.0). Lower values are more deterministic.")]
        [Range(0f, 2f)]
        [SerializeField]
        private float temperature = 0.8f;

        [Tooltip("Voice style (optional, depends on the voice)")]
        [SerializeField]
        private string style;

        [Tooltip("Voice pitch adjustment (optional)")]
        [SerializeField]
        private string pitch;

        [Tooltip("Voice speaking rate adjustment (optional)")]
        [SerializeField]
        private string rate;

        [Tooltip("Voice volume adjustment (optional)")]
        [SerializeField]
        private string volume;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the language/locale code for voice filtering.
        /// </summary>
        public string Language
        {
            get => language;
            set => language = value;
        }

        /// <summary>
        ///     Gets or sets the Azure Neural Voice name.
        /// </summary>
        public string VoiceName
        {
            get => voiceName;
            set => voiceName = value;
        }

        /// <summary>
        ///     Gets or sets the voice type.
        /// </summary>
        public string VoiceType
        {
            get => voiceType;
            set => voiceType = value;
        }

        /// <summary>
        ///     Gets or sets the temperature for response generation.
        /// </summary>
        public float Temperature
        {
            get => temperature;
            set => temperature = Mathf.Clamp(value, 0f, 2f);
        }

        /// <summary>
        ///     Gets or sets the voice style.
        /// </summary>
        public string Style
        {
            get => style;
            set => style = value;
        }

        /// <summary>
        ///     Gets or sets the voice pitch.
        /// </summary>
        public string Pitch
        {
            get => pitch;
            set => pitch = value;
        }

        /// <summary>
        ///     Gets or sets the voice speaking rate.
        /// </summary>
        public string Rate
        {
            get => rate;
            set => rate = value;
        }

        /// <summary>
        ///     Gets or sets the voice volume.
        /// </summary>
        public string Volume
        {
            get => volume;
            set => volume = value;
        }

        #endregion
    }
}
