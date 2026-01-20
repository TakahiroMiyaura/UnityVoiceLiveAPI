// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Audio
{
    /// <summary>
    ///     Unity audio playback component.
    ///     Plays audio responses from Azure AI VoiceLive API using Unity's AudioSource.
    /// </summary>
    public class UnityAudioPlayback : IDisposable
    {
        #region Nested Types

        /// <summary>
        ///     Represents pending PCM16 data to be converted to AudioClip on the main thread.
        /// </summary>
        private class PendingAudioData
        {
            public byte[] Pcm16Data { get; set; }
            public int SampleRate { get; set; }
            public int Channels { get; set; }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the UnityAudioPlayback class.
        /// </summary>
        /// <param name="audioSource">The Unity AudioSource component to use for playback.</param>
        public UnityAudioPlayback(AudioSource audioSource)
        {
            this.audioSource = audioSource ?? throw new ArgumentNullException(nameof(audioSource));
            clipQueue = new Queue<AudioClip>();
            pendingAudioQueue = new ConcurrentQueue<PendingAudioData>();
            isPlaying = false;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        ///     Releases all resources used by the UnityAudioPlayback.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                StopPlayback();
                ClearQueue();
                isDisposed = true;
            }
        }

        #endregion

        #region Private Fields

        private readonly AudioSource audioSource;
        private readonly Queue<AudioClip> clipQueue;
        private readonly ConcurrentQueue<PendingAudioData> pendingAudioQueue;
        private bool isPlaying;
        private bool isDisposed;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether audio is currently playing or pending to play.
        /// </summary>
        /// <remarks>
        ///     Uses Unity's implicit bool conversion to properly detect destroyed AudioSource.
        /// </remarks>
        public bool IsPlaying => audioSource &&
            (audioSource.isPlaying || clipQueue.Count > 0 || !pendingAudioQueue.IsEmpty);

        /// <summary>
        ///     Gets the number of audio clips in the playback queue.
        /// </summary>
        public int QueueCount => clipQueue.Count;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Enqueues an AudioClip for playback.
        /// </summary>
        /// <param name="clip">The AudioClip to enqueue.</param>
        public void EnqueueAudioClip(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Attempted to enqueue null AudioClip");
                return;
            }

            clipQueue.Enqueue(clip);
        }

        /// <summary>
        ///     Enqueues PCM16 audio data for playback.
        ///     This method is thread-safe and can be called from background threads.
        ///     The AudioClip will be created on the main thread in the next Update() call.
        /// </summary>
        /// <param name="pcm16Data">The PCM16 audio data as byte array.</param>
        /// <param name="sampleRate">The sample rate of the audio data (default: 24000 Hz).</param>
        /// <param name="channels">The number of audio channels (default: 1 for mono).</param>
        public void EnqueuePCM16Data(byte[] pcm16Data, int sampleRate = 24000, int channels = 1)
        {
            if (pcm16Data == null || pcm16Data.Length == 0)
            {
                Debug.LogWarning("Attempted to enqueue empty PCM16 data");
                return;
            }

            // Queue the raw data for main thread processing
            // AudioClip.Create must be called from the main thread
            pendingAudioQueue.Enqueue(new PendingAudioData
            {
                Pcm16Data = pcm16Data,
                SampleRate = sampleRate,
                Channels = channels
            });
        }

        /// <summary>
        ///     Plays the next audio clip in the queue if available.
        /// </summary>
        public void PlayNext()
        {
            // Check if AudioSource has been destroyed (Unity implicit bool conversion)
            if (!audioSource)
            {
                isPlaying = false;
                return;
            }

            if (clipQueue.Count == 0)
            {
                isPlaying = false;
                return;
            }

            if (audioSource.isPlaying)
            {
                // Already playing, wait for current clip to finish
                return;
            }

            var nextClip = clipQueue.Dequeue();
            audioSource.clip = nextClip;
            audioSource.Play();
            isPlaying = true;
        }

        /// <summary>
        ///     Updates the audio playback state.
        ///     Must be called regularly (e.g., from Unity's Update() method).
        ///     This method processes pending audio data on the main thread.
        /// </summary>
        public void Update()
        {
            // Check if AudioSource has been destroyed (Unity implicit bool conversion)
            if (!audioSource)
            {
                return;
            }

            // Process pending audio data on the main thread
            // AudioClip.Create must be called from the main thread
            ProcessPendingAudioData();

            // Auto-start playback if we have clips and not playing
            if (!audioSource.isPlaying && clipQueue.Count > 0)
            {
                PlayNext();
            }
            else if (!audioSource.isPlaying && clipQueue.Count == 0 && pendingAudioQueue.IsEmpty)
            {
                isPlaying = false;
            }
        }

        /// <summary>
        ///     Processes pending audio data and creates AudioClips on the main thread.
        /// </summary>
        private void ProcessPendingAudioData()
        {
            // Process up to 10 pending items per frame to avoid blocking
            var processed = 0;
            while (processed < 10 && pendingAudioQueue.TryDequeue(out var pendingData))
            {
                var clip = CreateAudioClipFromPCM16(
                    pendingData.Pcm16Data,
                    pendingData.SampleRate,
                    pendingData.Channels);

                if (clip != null)
                {
                    EnqueueAudioClip(clip);
                }

                processed++;
            }
        }

        /// <summary>
        ///     Starts playback if not already playing.
        /// </summary>
        public void StartPlayback()
        {
            if (!isPlaying && clipQueue.Count > 0)
            {
                PlayNext();
            }
        }

        /// <summary>
        ///     Stops playback and clears the audio queue.
        /// </summary>
        public void StopPlayback()
        {
            // Check if AudioSource has been destroyed (Unity implicit bool conversion)
            if (audioSource && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            isPlaying = false;
        }

        /// <summary>
        ///     Clears all audio clips from the playback queue.
        /// </summary>
        public void ClearQueue()
        {
            // Clear pending audio data queue
            while (pendingAudioQueue.TryDequeue(out _))
            {
                // Just dequeue and discard
            }

            // Destroy all queued clips to free memory
            while (clipQueue.Count > 0)
            {
                var clip = clipQueue.Dequeue();
                if (clip != null)
                {
                    Object.Destroy(clip);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates an AudioClip from PCM16 audio data.
        /// </summary>
        /// <param name="pcm16Data">The PCM16 audio data as byte array.</param>
        /// <param name="sampleRate">The sample rate of the audio data.</param>
        /// <param name="channels">The number of audio channels.</param>
        /// <returns>The created AudioClip, or null if creation failed.</returns>
        private AudioClip CreateAudioClipFromPCM16(byte[] pcm16Data, int sampleRate, int channels)
        {
            try
            {
                // Convert PCM16 to float samples
                var samples = ConvertPCM16ToFloat(pcm16Data);

                // Create AudioClip
                var sampleCount = samples.Length / channels;
                var clip = AudioClip.Create(
                    $"VoiceLiveResponse_{DateTime.Now.Ticks}",
                    sampleCount,
                    channels,
                    sampleRate,
                    false);

                // Set audio data
                if (!clip.SetData(samples, 0))
                {
                    Debug.LogError("Failed to set audio data on AudioClip");
                    Object.Destroy(clip);
                    return null;
                }

                return clip;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create AudioClip from PCM16 data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        ///     Converts PCM16 byte array to float samples.
        /// </summary>
        /// <param name="pcm16Data">The PCM16 audio data as byte array.</param>
        /// <returns>The float audio samples (-1.0 to 1.0).</returns>
        private float[] ConvertPCM16ToFloat(byte[] pcm16Data)
        {
            var sampleCount = pcm16Data.Length / 2;
            var samples = new float[sampleCount];

            for (var i = 0; i < sampleCount; i++)
            {
                // Read little-endian 16-bit signed integer
                var pcm16Sample = (short)(pcm16Data[i * 2] | (pcm16Data[i * 2 + 1] << 8));

                // Convert to float [-1.0, 1.0]
                samples[i] = pcm16Sample / (float)short.MaxValue;
            }

            return samples;
        }

        #endregion
    }
}