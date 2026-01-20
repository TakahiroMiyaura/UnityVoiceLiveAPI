// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for video output in VoiceLive avatar sessions.
    /// </summary>
    [Serializable]
    public class VideoSettings
    {
        #region Fields

        [Header("Resolution")]
        [Tooltip("Video width in pixels (e.g., 1920 for 1080p, 1280 for 720p)")]
        [SerializeField]
        private int width = 1920;

        [Tooltip("Video height in pixels (e.g., 1080 for 1080p, 720 for 720p)")]
        [SerializeField]
        private int height = 1080;

        [Header("Encoding")]
        [Tooltip("Video bitrate in kbps")]
        [SerializeField]
        private int bitRate = 2000000;

        [Tooltip("Video codec for encoding. Select from dropdown or choose 'Custom...' to enter a custom value.")]
        [VoiceLiveDropdown("Codecs")]
        [SerializeField]
        private string codec = "h264";

        [Header("Crop Settings")]
        [Tooltip("Enable video cropping")]
        [SerializeField]
        private bool enableCrop;

        [Tooltip("Crop top-left X coordinate")]
        [SerializeField]
        private int cropTopLeftX;

        [Tooltip("Crop top-left Y coordinate")]
        [SerializeField]
        private int cropTopLeftY;

        [Tooltip("Crop bottom-right X coordinate")]
        [SerializeField]
        private int cropBottomRightX;

        [Tooltip("Crop bottom-right Y coordinate")]
        [SerializeField]
        private int cropBottomRightY;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the video width in pixels.
        /// </summary>
        public int Width
        {
            get => width;
            set => width = Mathf.Max(1, value);
        }

        /// <summary>
        ///     Gets or sets the video height in pixels.
        /// </summary>
        public int Height
        {
            get => height;
            set => height = Mathf.Max(1, value);
        }

        /// <summary>
        ///     Gets or sets the video bitrate in kbps.
        /// </summary>
        public int BitRate
        {
            get => bitRate;
            set => bitRate = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets the video codec (API value string).
        /// </summary>
        public string Codec
        {
            get => codec;
            set => codec = value;
        }

        /// <summary>
        ///     Gets or sets whether video cropping is enabled.
        /// </summary>
        public bool EnableCrop
        {
            get => enableCrop;
            set => enableCrop = value;
        }

        /// <summary>
        ///     Gets or sets the crop top-left X coordinate.
        /// </summary>
        public int CropTopLeftX
        {
            get => cropTopLeftX;
            set => cropTopLeftX = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets the crop top-left Y coordinate.
        /// </summary>
        public int CropTopLeftY
        {
            get => cropTopLeftY;
            set => cropTopLeftY = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets the crop bottom-right X coordinate.
        /// </summary>
        public int CropBottomRightX
        {
            get => cropBottomRightX;
            set => cropBottomRightX = Mathf.Max(0, value);
        }

        /// <summary>
        ///     Gets or sets the crop bottom-right Y coordinate.
        /// </summary>
        public int CropBottomRightY
        {
            get => cropBottomRightY;
            set => cropBottomRightY = Mathf.Max(0, value);
        }

        #endregion
    }
}
