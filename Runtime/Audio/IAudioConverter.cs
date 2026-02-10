// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Interface for audio format conversion.
    ///     Implementations convert float audio samples to specific output formats.
    /// </summary>
    public interface IAudioConverter
    {
        /// <summary>
        ///     Gets the output format identifier (e.g., "pcm16", "pcm24", "opus").
        /// </summary>
        string OutputFormat { get; }

        /// <summary>
        ///     Converts float audio samples to the target format.
        /// </summary>
        /// <param name="floatSamples">The float audio samples (-1.0 to 1.0).</param>
        /// <param name="sampleCount">The number of samples to convert.</param>
        /// <returns>The converted audio data as byte array.</returns>
        byte[] Convert(float[] floatSamples, int sampleCount);
    }
}
