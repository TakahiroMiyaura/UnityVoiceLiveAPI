// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for avatar background in VoiceLive sessions.
    /// </summary>
    [Serializable]
    public class BackgroundSettings
    {
        #region Fields

        [Header("Background")]
        [Tooltip("Background color in hex format (e.g., '#00FF00FF' for green with alpha)")]
        [SerializeField]
        private string color = "#00FF00FF";

        [Tooltip("URL to a background image (optional, overrides color if set)")]
        [SerializeField]
        private string imageUrl;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the background color in hex format.
        /// </summary>
        public string Color
        {
            get => color;
            set => color = value;
        }

        /// <summary>
        ///     Gets or sets the background image URL.
        /// </summary>
        public string ImageUrl
        {
            get => imageUrl;
            set => imageUrl = value;
        }

        /// <summary>
        ///     Gets whether an image URL is specified.
        /// </summary>
        public bool HasImageUrl => !string.IsNullOrEmpty(imageUrl);

        #endregion
    }
}
