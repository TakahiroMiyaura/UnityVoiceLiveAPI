// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Converts float audio samples to PCM16 (16-bit signed integer) format.
    ///     This is the default converter for Azure AI VoiceLive API.
    /// </summary>
    public class PCM16Converter : IAudioConverter
    {
        /// <summary>
        ///     Gets the output format identifier.
        /// </summary>
        public string OutputFormat => "pcm16";

        /// <summary>
        ///     Converts float audio samples to PCM16 format.
        /// </summary>
        /// <param name="floatSamples">The float audio samples (-1.0 to 1.0).</param>
        /// <param name="sampleCount">The number of samples to convert.</param>
        /// <returns>The PCM16 audio data as byte array (2 bytes per sample, little-endian).</returns>
        public byte[] Convert(float[] floatSamples, int sampleCount)
        {
            var pcm16Data = new byte[sampleCount * 2]; // 2 bytes per sample (16-bit)

            for (var i = 0; i < sampleCount; i++)
            {
                // Clamp float to [-1.0, 1.0]
                var sample = Mathf.Clamp(floatSamples[i], -1.0f, 1.0f);

                // Convert to 16-bit signed integer
                var pcm16Sample = (short)(sample * short.MaxValue);

                // Write as little-endian bytes
                pcm16Data[i * 2] = (byte)(pcm16Sample & 0xFF);
                pcm16Data[i * 2 + 1] = (byte)((pcm16Sample >> 8) & 0xFF);
            }

            return pcm16Data;
        }
    }
}
