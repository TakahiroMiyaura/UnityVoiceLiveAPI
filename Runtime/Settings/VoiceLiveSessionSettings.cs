// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Specifies the connection mode for VoiceLive API.
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        ///     AI Agent mode - connects to a configured AI Agent.
        ///     Instructions and Model are managed by the Agent, not sent via session.update.
        /// </summary>
        AIAgent = 0,

        /// <summary>
        ///     AI Model mode - connects directly to an AI model.
        ///     Instructions and Model can be configured via session.update.
        /// </summary>
        AIModel = 1
    }

    /// <summary>
    ///     ScriptableObject containing all VoiceLive session configuration settings.
    /// </summary>
    /// <remarks>
    ///     Create instances of this asset via Assets > Create > VoiceLive API > Session Settings.
    ///     Assign to VoiceLiveAgentClient to configure session parameters.
    /// </remarks>
    public class VoiceLiveSessionSettings : ScriptableObject
    {
        #region Fields

        [Tooltip("Connection mode: AIAgent (uses configured Agent) or AIModel (direct model connection)")]
        [SerializeField]
        private ConnectionMode connectionMode = ConnectionMode.AIAgent;

        [Tooltip("AI model to use. Only applicable in AIModel mode.")]
        [VoiceLiveDropdown("Models")]
        [SerializeField]
        private string model = "gpt-realtime";

        [Tooltip("Instructions for the AI model (system prompt). Only used in AIModel mode.")]
        [TextArea(3, 10)]
        [SerializeField]
        private string instructions = "You are a helpful AI assistant.";

        [SerializeField]
        private VoiceSettings voice = new VoiceSettings();

        [SerializeField]
        private AudioProcessingSettings audioProcessing = new AudioProcessingSettings();

        [SerializeField]
        private TurnDetectionSettings turnDetection = new TurnDetectionSettings();

        [Tooltip("Avatar settings ScriptableObject. If null, avatar is disabled for this session.")]
        [SerializeField]
        private VoiceLiveAvatarSettings avatarSettings;

        [SerializeField]
        private AnimationSettings animation = new AnimationSettings();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the connection mode (AIAgent or AIModel).
        /// </summary>
        public ConnectionMode ConnectionMode
        {
            get => connectionMode;
            set => connectionMode = value;
        }

        /// <summary>
        ///     Gets a value indicating whether this is configured for AI Agent mode.
        /// </summary>
        public bool IsAgentMode => connectionMode == ConnectionMode.AIAgent;

        /// <summary>
        ///     Gets a value indicating whether this is configured for AI Model mode.
        /// </summary>
        public bool IsModelMode => connectionMode == ConnectionMode.AIModel;

        /// <summary>
        ///     Gets or sets the AI model to use (only applicable in AIModel mode).
        /// </summary>
        public string Model
        {
            get => model;
            set => model = value;
        }

        /// <summary>
        ///     Gets the model name string for API calls.
        /// </summary>
        public string ModelName => model;

        /// <summary>
        ///     Gets or sets the instructions for the AI model.
        ///     Note: Only used in AIModel mode. In AIAgent mode, instructions are managed by the Agent.
        /// </summary>
        public string Instructions
        {
            get => instructions;
            set => instructions = value;
        }

        /// <summary>
        ///     Gets or sets the voice settings.
        /// </summary>
        public VoiceSettings Voice
        {
            get => voice;
            set => voice = value ?? new VoiceSettings();
        }

        /// <summary>
        ///     Gets or sets the audio processing settings.
        /// </summary>
        public AudioProcessingSettings AudioProcessing
        {
            get => audioProcessing;
            set => audioProcessing = value ?? new AudioProcessingSettings();
        }

        /// <summary>
        ///     Gets or sets the turn detection settings.
        /// </summary>
        public TurnDetectionSettings TurnDetection
        {
            get => turnDetection;
            set => turnDetection = value ?? new TurnDetectionSettings();
        }

        /// <summary>
        ///     Gets a value indicating whether avatar is enabled (AvatarSettings is assigned).
        /// </summary>
        public bool HasAvatar => avatarSettings != null;

        /// <summary>
        ///     Gets or sets the avatar settings ScriptableObject.
        ///     If null, avatar functionality is disabled for this session.
        /// </summary>
        public VoiceLiveAvatarSettings AvatarSettings
        {
            get => avatarSettings;
            set => avatarSettings = value;
        }

        /// <summary>
        ///     Gets or sets the animation settings.
        /// </summary>
        public AnimationSettings Animation
        {
            get => animation;
            set => animation = value ?? new AnimationSettings();
        }

        #endregion
    }
}
