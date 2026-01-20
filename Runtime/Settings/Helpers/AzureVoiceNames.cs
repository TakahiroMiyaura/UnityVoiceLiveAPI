// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Provides constants for commonly used Azure Neural Voice names.
    /// </summary>
    /// <remarks>
    ///     Azure offers 600+ neural voices across 150+ languages.
    ///     This class provides constants for frequently used voices.
    ///     For the complete list, see:
    ///     https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support
    /// </remarks>
    public static class AzureVoiceNames
    {
        #region Japanese Voices

        /// <summary>
        ///     Japanese female voice - Nanami (HD quality).
        /// </summary>
        public const string JaJPNanamiHD = "ja-JP-Nanami:DragonHDLatestNeural";

        /// <summary>
        ///     Japanese male voice - Keita (HD quality).
        /// </summary>
        public const string JaJPKeitaHD = "ja-JP-Keita:DragonHDLatestNeural";

        /// <summary>
        ///     Japanese female voice - Nanami (Standard).
        /// </summary>
        public const string JaJPNanami = "ja-JP-NanamiNeural";

        /// <summary>
        ///     Japanese male voice - Keita (Standard).
        /// </summary>
        public const string JaJPKeita = "ja-JP-KeitaNeural";

        #endregion

        #region English (US) Voices

        /// <summary>
        ///     US English female voice - Ava (HD quality).
        /// </summary>
        public const string EnUSAvaHD = "en-US-Ava:DragonHDLatestNeural";

        /// <summary>
        ///     US English male voice - Andrew (HD quality).
        /// </summary>
        public const string EnUSAndrewHD = "en-US-Andrew:DragonHDLatestNeural";

        /// <summary>
        ///     US English female voice - Jenny (Standard).
        /// </summary>
        public const string EnUSJenny = "en-US-JennyNeural";

        /// <summary>
        ///     US English male voice - Guy (Standard).
        /// </summary>
        public const string EnUSGuy = "en-US-GuyNeural";

        #endregion

        #region English (UK) Voices

        /// <summary>
        ///     UK English female voice - Sonia (Standard).
        /// </summary>
        public const string EnGBSonia = "en-GB-SoniaNeural";

        /// <summary>
        ///     UK English male voice - Ryan (Standard).
        /// </summary>
        public const string EnGBRyan = "en-GB-RyanNeural";

        #endregion

        #region Chinese Voices

        /// <summary>
        ///     Chinese (Mandarin) female voice - Xiaoxiao (Standard).
        /// </summary>
        public const string ZhCNXiaoxiao = "zh-CN-XiaoxiaoNeural";

        /// <summary>
        ///     Chinese (Mandarin) male voice - Yunxi (Standard).
        /// </summary>
        public const string ZhCNYunxi = "zh-CN-YunxiNeural";

        #endregion

        #region German Voices

        /// <summary>
        ///     German female voice - Katja (Standard).
        /// </summary>
        public const string DeDeKatja = "de-DE-KatjaNeural";

        /// <summary>
        ///     German male voice - Conrad (Standard).
        /// </summary>
        public const string DeDeConrad = "de-DE-ConradNeural";

        #endregion

        #region French Voices

        /// <summary>
        ///     French female voice - Denise (Standard).
        /// </summary>
        public const string FrFRDenise = "fr-FR-DeniseNeural";

        /// <summary>
        ///     French male voice - Henri (Standard).
        /// </summary>
        public const string FrFRHenri = "fr-FR-HenriNeural";

        #endregion

        #region Spanish Voices

        /// <summary>
        ///     Spanish (Spain) female voice - Elvira (Standard).
        /// </summary>
        public const string EsESElvira = "es-ES-ElviraNeural";

        /// <summary>
        ///     Spanish (Spain) male voice - Alvaro (Standard).
        /// </summary>
        public const string EsESAlvaro = "es-ES-AlvaroNeural";

        #endregion

        #region Korean Voices

        /// <summary>
        ///     Korean female voice - SunHi (Standard).
        /// </summary>
        public const string KoKRSunHi = "ko-KR-SunHiNeural";

        /// <summary>
        ///     Korean male voice - InJoon (Standard).
        /// </summary>
        public const string KoKRInJoon = "ko-KR-InJoonNeural";

        #endregion
    }
}
