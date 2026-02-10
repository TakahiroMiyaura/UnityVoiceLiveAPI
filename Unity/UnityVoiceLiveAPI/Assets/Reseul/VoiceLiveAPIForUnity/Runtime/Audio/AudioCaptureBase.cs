// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Abstract base class for audio capture implementations.
    ///     Provides common functionality for buffering, conversion, and event handling.
    /// </summary>
    public abstract class AudioCaptureBase : IAudioCapture
    {
        #region Fields

        /// <summary>
        ///     The audio converter used to convert float samples to the target format.
        /// </summary>
        protected readonly IAudioConverter converter;

        /// <summary>
        ///     Buffer for storing float audio samples.
        /// </summary>
        protected float[] audioBuffer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the AudioCaptureBase class.
        /// </summary>
        /// <param name="sampleRate">The sample rate for audio capture (default: 24000 Hz for Azure AI).</param>
        /// <param name="converter">The audio converter to use (default: PCM16Converter).</param>
        protected AudioCaptureBase(int sampleRate = 24000, IAudioConverter converter = null)
        {
            SampleRate = sampleRate;
            this.converter = converter ?? new PCM16Converter();
            audioBuffer = new float[sampleRate];
        }

        #endregion

        #region IAudioCapture Implementation

        /// <inheritdoc />
        public event Action<byte[]> OnAudioDataCaptured;

        /// <inheritdoc />
        public bool IsCapturing { get; protected set; }

        /// <inheritdoc />
        public int SampleRate { get; }

        /// <inheritdoc />
        public abstract void StartCapture(string deviceName = null);

        /// <inheritdoc />
        public abstract void StopCapture();

        /// <inheritdoc />
        public abstract void Update();

        #endregion

        #region IDisposable Implementation

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases resources used by the audio capture.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopCapture();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Ensures the audio buffer has sufficient capacity.
        /// </summary>
        /// <param name="requiredSize">The minimum required buffer size.</param>
        protected void EnsureBufferCapacity(int requiredSize)
        {
            if (audioBuffer.Length < requiredSize)
            {
                audioBuffer = new float[requiredSize];
            }
        }

        /// <summary>
        ///     Converts float samples and raises the OnAudioDataCaptured event.
        /// </summary>
        /// <param name="floatSamples">The float audio samples to convert.</param>
        /// <param name="sampleCount">The number of samples to process.</param>
        protected void ProcessAndRaiseAudioData(float[] floatSamples, int sampleCount)
        {
            if (sampleCount <= 0)
            {
                return;
            }

            var convertedData = converter.Convert(floatSamples, sampleCount);
            RaiseAudioDataCaptured(convertedData);
        }

        /// <summary>
        ///     Raises the OnAudioDataCaptured event with the specified audio data.
        /// </summary>
        /// <param name="audioData">The converted audio data.</param>
        protected void RaiseAudioDataCaptured(byte[] audioData)
        {
            OnAudioDataCaptured?.Invoke(audioData);
        }

        #endregion
    }
}
