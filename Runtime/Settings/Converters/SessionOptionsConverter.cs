// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Core;
using CoreParts = Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Commons.Messages.Parts;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings.Converters
{
    /// <summary>
    ///     Converts Unity ScriptableObject settings to Core VoiceLiveSessionOptions.
    /// </summary>
    public static class SessionOptionsConverter
    {
        #region Public Methods

        /// <summary>
        ///     Converts VoiceLiveSessionSettings to VoiceLiveSessionOptions.
        /// </summary>
        /// <param name="settings">The Unity settings to convert.</param>
        /// <returns>A VoiceLiveSessionOptions instance configured from the settings.</returns>
        /// <remarks>
        ///     The conversion behavior depends on the ConnectionMode property of the settings:
        ///     <list type="bullet">
        ///         <item>
        ///             <term>AIAgent mode</term>
        ///             <description>Excludes Model and Instructions (managed by the Agent)</description>
        ///         </item>
        ///         <item>
        ///             <term>AIModel mode</term>
        ///             <description>Includes Model and Instructions for direct model configuration</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public static VoiceLiveSessionOptions ToSessionOptions(this VoiceLiveSessionSettings settings)
        {
            if (settings == null)
            {
                return VoiceLiveSessionOptions.CreateDefault();
            }

            var options = VoiceLiveSessionOptions.CreateDefault();

            if (settings.IsAgentMode)
            {
                // Clear Model for Agent sessions - Agent has its own model configuration
                // Setting to null ensures it won't be serialized (JsonIgnoreCondition.WhenWritingNull)
                options.Model = null;

                // Clear Instructions for Agent sessions - Agent API rejects session.update with instructions
                options.Instructions = null;

                // Explicitly set Avatar to null for AIAgent mode (matches ConsoleApp behavior)
                // Avatar is only used in Avatar mode, not in pure AIAgent mode
                if (!settings.HasAvatar)
                {
                    options.Avatar = null;
                }
            }
            else
            {
                // AI Model mode: Apply model and instructions from settings
                options.Model = settings.ModelName;

                if (!string.IsNullOrEmpty(settings.Instructions))
                {
                    options.Instructions = settings.Instructions;
                }

                // AI Model mode without avatar also needs Avatar = null
                if (!settings.HasAvatar)
                {
                    options.Avatar = null;
                }
            }

            // Apply voice settings
            options.Voice = settings.Voice.ToVoice();

            // Apply audio processing settings
            ApplyAudioProcessingSettings(options, settings.AudioProcessing);

            // Apply turn detection settings
            options.TurnDetection = settings.TurnDetection.ToTurnDetection();

            // Apply animation settings
            options.Animation = settings.Animation.ToAnimation();

            // Apply avatar settings if assigned (for Avatar mode)
            if (settings.HasAvatar)
            {
                options.Avatar = settings.AvatarSettings.ToAvatar();
            }

            return options;
        }

        /// <summary>
        ///     Converts VoiceSettings to Core Voice.
        /// </summary>
        /// <param name="settings">The voice settings to convert.</param>
        /// <returns>A Voice instance.</returns>
        public static CoreParts.Voice ToVoice(this VoiceSettings settings)
        {
            if (settings == null)
            {
                return new CoreParts.Voice
                {
                    Name = "ja-JP-Nanami:DragonHDLatestNeural",
                    Type = "azure-standard"
                };
            }

            // Note: Temperature is not set for Agent sessions to match ConsoleApp behavior
            // Agent API may reject session.update with voice.temperature field
            var voice = new CoreParts.Voice
            {
                Name = settings.VoiceName,
                Type = settings.VoiceType
            };

            // Apply optional voice parameters
            if (!string.IsNullOrEmpty(settings.Style))
            {
                voice.Style = settings.Style;
            }

            if (!string.IsNullOrEmpty(settings.Pitch))
            {
                voice.Pitch = settings.Pitch;
            }

            if (!string.IsNullOrEmpty(settings.Rate))
            {
                voice.Rate = settings.Rate;
            }

            if (!string.IsNullOrEmpty(settings.Volume))
            {
                voice.Volume = settings.Volume;
            }

            return voice;
        }

        /// <summary>
        ///     Converts TurnDetectionSettings to Core TurnDetection.
        /// </summary>
        /// <param name="settings">The turn detection settings to convert.</param>
        /// <returns>A TurnDetection instance.</returns>
        public static CoreParts.TurnDetection ToTurnDetection(this TurnDetectionSettings settings)
        {
            if (settings == null)
            {
                return new CoreParts.TurnDetection
                {
                    Type = "server_vad",
                    Threshold = 0.5f,
                    SilenceDurationMs = 500,
                    CreateResponse = true
                };
            }

            // Note: Only include fields that ConsoleApp sends to match behavior
            // Agent API may reject session.update with extra turn_detection fields
            // (prefix_padding_ms, interrupt_response, remove_filler_words)
            return new CoreParts.TurnDetection
            {
                Type = settings.Type,
                Threshold = settings.Threshold,
                SilenceDurationMs = settings.SilenceDurationMs,
                CreateResponse = settings.CreateResponse
            };
        }

        /// <summary>
        ///     Converts AnimationSettings to Core Animation.
        /// </summary>
        /// <param name="settings">The animation settings to convert.</param>
        /// <returns>An Animation instance.</returns>
        /// <remarks>
        ///     If no outputs are enabled, returns default ["viseme_id"] to match ConsoleApp behavior.
        ///     The API may reject session.update with empty or null animation outputs.
        /// </remarks>
        public static CoreParts.Animation ToAnimation(this AnimationSettings settings)
        {
            if (settings == null)
            {
                return new CoreParts.Animation
                {
                    Outputs = new[] { "viseme_id" }
                };
            }

            var outputs = settings.GetOutputTypes();
            return new CoreParts.Animation
            {
                // If no outputs are enabled, use default to match ConsoleApp behavior
                Outputs = outputs.Count > 0 ? outputs.ToArray() : new[] { "viseme_id" }
            };
        }

        /// <summary>
        ///     Converts VoiceLiveAvatarSettings to Core Avatar.
        /// </summary>
        /// <param name="settings">The avatar settings to convert.</param>
        /// <returns>An Avatar instance.</returns>
        public static CoreParts.Avatar ToAvatar(this VoiceLiveAvatarSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            return new CoreParts.Avatar
            {
                Character = settings.EffectiveCharacterName,
                Style = settings.EffectiveStyleName,
                Video = settings.Video.ToVideo(settings.Background)
            };
        }

        /// <summary>
        ///     Converts VideoSettings and BackgroundSettings to Core Video.
        /// </summary>
        /// <param name="settings">The video settings to convert.</param>
        /// <param name="backgroundSettings">The background settings to convert.</param>
        /// <returns>A Video instance.</returns>
        public static CoreParts.Video ToVideo(this VideoSettings settings, BackgroundSettings backgroundSettings = null)
        {
            if (settings == null)
            {
                return new CoreParts.Video
                {
                    Resolution = new CoreParts.Resolution { Width = 1920, Height = 1080 },
                    Codec = "h264",
                    BitRate = 2000000
                };
            }

            var video = new CoreParts.Video
            {
                Resolution = new CoreParts.Resolution
                {
                    Width = settings.Width,
                    Height = settings.Height
                },
                Codec = settings.Codec,
                BitRate = settings.BitRate
            };

            // Apply crop settings if enabled
            if (settings.EnableCrop)
            {
                video.Crop = new CoreParts.Crop
                {
                    TopLeft = new[] { settings.CropTopLeftX, settings.CropTopLeftY },
                    BottomRight = new[] { settings.CropBottomRightX, settings.CropBottomRightY }
                };
            }

            // Apply background settings
            if (backgroundSettings != null)
            {
                video.Background = backgroundSettings.ToBackground();
            }

            return video;
        }

        /// <summary>
        ///     Converts BackgroundSettings to Core Background.
        /// </summary>
        /// <param name="settings">The background settings to convert.</param>
        /// <returns>A Background instance.</returns>
        public static CoreParts.Background ToBackground(this BackgroundSettings settings)
        {
            if (settings == null)
            {
                return new CoreParts.Background
                {
                    Color = "#00FF00FF"
                };
            }

            var background = new CoreParts.Background
            {
                Color = settings.Color
            };

            if (settings.HasImageUrl)
            {
                background.ImageUrl = settings.ImageUrl;
            }

            return background;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Applies audio processing settings to the session options.
        /// </summary>
        /// <param name="options">The options to modify.</param>
        /// <param name="settings">The audio processing settings.</param>
        private static void ApplyAudioProcessingSettings(VoiceLiveSessionOptions options,
            AudioProcessingSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            options.InputAudioFormat = settings.InputAudioFormat;
            options.OutputAudioFormat = settings.OutputAudioFormat;
            options.InputAudioSamplingRate = settings.SampleRate;

            // Apply noise reduction (skip if "none" or empty)
            var noiseReductionType = settings.NoiseReduction;
            if (!string.IsNullOrEmpty(noiseReductionType) &&
                !string.Equals(noiseReductionType, "none", System.StringComparison.OrdinalIgnoreCase))
            {
                options.InputAudioNoiseReduction = new CoreParts.AudioInputAudioNoiseReductionSettings
                {
                    Type = noiseReductionType
                };
            }
            else
            {
                options.InputAudioNoiseReduction = null;
            }

            // Apply echo cancellation
            if (settings.EnableEchoCancellation)
            {
                options.InputAudioEchoCancellation = new CoreParts.AudioInputEchoCancellationSettings
                {
                    Type = "azure"
                };
            }
            else
            {
                options.InputAudioEchoCancellation = null;
            }
        }

        #endregion
    }
}
