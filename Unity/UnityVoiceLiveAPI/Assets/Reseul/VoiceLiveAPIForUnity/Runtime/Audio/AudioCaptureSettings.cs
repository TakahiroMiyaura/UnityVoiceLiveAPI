// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Abstract base class for audio capture settings.
    ///     Derive from this class to create custom audio capture configurations
    ///     that can be assigned via Inspector.
    /// </summary>
    /// <remarks>
    ///     Create concrete implementations as ScriptableObjects to define
    ///     audio capture configurations for different devices (e.g., Unity Microphone, XR devices).
    ///     Assign the created asset to UnityVoiceLiveClient's Audio Capture Settings field.
    /// </remarks>
    public abstract class AudioCaptureSettings : ScriptableObject
    {
        /// <summary>
        ///     Creates an audio capture instance based on the settings.
        /// </summary>
        /// <param name="sampleRate">The sample rate for audio capture.</param>
        /// <returns>A new IAudioCapture instance configured according to these settings.</returns>
        public abstract IAudioCapture CreateAudioCapture(int sampleRate);

        /// <summary>
        ///     Gets a description of this audio capture configuration.
        ///     Used for display in logs and debugging.
        /// </summary>
        public virtual string Description => GetType().Name;
    }
}
