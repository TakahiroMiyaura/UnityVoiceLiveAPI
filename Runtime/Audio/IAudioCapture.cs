// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Interface for audio capture implementations.
    ///     Defines the contract for capturing audio from various sources (microphones, XR devices, etc.).
    /// </summary>
    public interface IAudioCapture : IDisposable
    {
        /// <summary>
        ///     Fired when audio data is captured and converted.
        /// </summary>
        event Action<byte[]> OnAudioDataCaptured;

        /// <summary>
        ///     Gets a value indicating whether audio capture is active.
        /// </summary>
        bool IsCapturing { get; }

        /// <summary>
        ///     Gets the sample rate of the captured audio.
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        ///     Starts capturing audio.
        /// </summary>
        /// <param name="deviceName">The name of the audio device (null for default device).</param>
        void StartCapture(string deviceName = null);

        /// <summary>
        ///     Stops capturing audio.
        /// </summary>
        void StopCapture();

        /// <summary>
        ///     Updates the audio capture and processes new audio data.
        ///     Must be called regularly (e.g., from Unity's Update() method).
        /// </summary>
        void Update();
    }
}
