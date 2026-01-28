// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     ScriptableObject containing all VoiceLive avatar configuration settings.
    /// </summary>
    /// <remarks>
    ///     Create instances of this asset via Assets > Create > VoiceLive API > Avatar Settings.
    ///     Assign to VoiceLiveSessionSettings to enable avatar functionality.
    ///     If no avatar settings are assigned, avatar will be disabled for the session.
    ///     Avatar presets define valid character:style combinations supported by the Azure AI Avatar service.
    /// </remarks>
    [CreateAssetMenu(fileName = "VoiceLiveAvatarSettings", menuName = "Microsoft Foundry/VoiceLive API/Avatar Settings", order = 3)]
    public class VoiceLiveAvatarSettings : ScriptableObject
    {
        #region Fields

        [Header("Avatar Preset")]
        [Tooltip("Avatar character and style preset. Only predefined combinations are supported by Azure AI Avatar.")]
        [VoiceLiveDropdown("AvatarPresets")]
        [SerializeField]
        private string avatarPreset = "lisa:casual-sitting";

        [Header("Video Settings")]
        [SerializeField]
        private VideoSettings video = new VideoSettings();

        [Header("Background Settings")]
        [SerializeField]
        private BackgroundSettings background = new BackgroundSettings();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the avatar preset (character:style combination).
        /// </summary>
        public string AvatarPreset
        {
            get => avatarPreset;
            set => avatarPreset = value;
        }

        /// <summary>
        ///     Gets the avatar character name parsed from the preset.
        /// </summary>
        public string Character
        {
            get
            {
                if (string.IsNullOrEmpty(avatarPreset))
                {
                    return "lisa";
                }

                var parts = avatarPreset.Split(':');
                return parts.Length > 0 ? parts[0] : "lisa";
            }
        }

        /// <summary>
        ///     Gets the avatar style name parsed from the preset.
        /// </summary>
        public string Style
        {
            get
            {
                if (string.IsNullOrEmpty(avatarPreset))
                {
                    return "casual-sitting";
                }

                var parts = avatarPreset.Split(':');
                return parts.Length > 1 ? parts[1] : "casual-sitting";
            }
        }

        /// <summary>
        ///     Gets or sets the video settings.
        /// </summary>
        public VideoSettings Video
        {
            get => video;
            set => video = value ?? new VideoSettings();
        }

        /// <summary>
        ///     Gets or sets the background settings.
        /// </summary>
        public BackgroundSettings Background
        {
            get => background;
            set => background = value ?? new BackgroundSettings();
        }

        /// <summary>
        ///     Gets the effective character name for API calls.
        /// </summary>
        public string EffectiveCharacterName => Character;

        /// <summary>
        ///     Gets the effective style name for API calls.
        /// </summary>
        public string EffectiveStyleName => Style;

        #endregion
    }
}
