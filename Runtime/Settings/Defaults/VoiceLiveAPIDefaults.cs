// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     ScriptableObject containing default values for VoiceLive API settings.
    ///     These values are used to populate dropdown menus in the Inspector.
    /// </summary>
    /// <remarks>
    ///     A default asset is included with the package. Users can create their own
    ///     VoiceLiveAPIDefaults asset to customize available options without modifying code.
    /// </remarks>
    [CreateAssetMenu(fileName = "VoiceLiveAPIDefaults", menuName = "Microsoft Foundry/API Defaults", order = 200)]
    public class VoiceLiveAPIDefaults : ScriptableObject
    {
        #region Singleton

        private static VoiceLiveAPIDefaults instance;
        private static VoiceLiveAPIDefaults explicitInstance;

        /// <summary>
        ///     Gets the singleton instance of VoiceLiveAPIDefaults.
        ///     If an explicit instance is set via <see cref="SetInstance"/>, it will be used.
        ///     Otherwise, searches for the asset in Resources folder first, then in all loaded assets.
        /// </summary>
        public static VoiceLiveAPIDefaults Instance
        {
            get
            {
                // Use explicitly set instance if available
                if (explicitInstance != null)
                {
                    return explicitInstance;
                }

                if (instance == null)
                {
                    // Try to load from Resources first
                    instance = Resources.Load<VoiceLiveAPIDefaults>("VoiceLiveAPIDefaults");

                    // If not found, search all assets (Editor only)
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        var guids = UnityEditor.AssetDatabase.FindAssets("t:VoiceLiveAPIDefaults");
                        if (guids.Length > 0)
                        {
                            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                            instance = UnityEditor.AssetDatabase.LoadAssetAtPath<VoiceLiveAPIDefaults>(path);
                        }
                    }
#endif

                    // If still not found, create a runtime instance with defaults
                    if (instance == null)
                    {
                        instance = CreateInstance<VoiceLiveAPIDefaults>();
                        instance.name = "VoiceLiveAPIDefaults (Runtime)";
                    }
                }

                return instance;
            }
        }

        /// <summary>
        ///     Sets an explicit instance to use as the defaults.
        ///     This takes priority over auto-discovered instances.
        /// </summary>
        /// <param name="defaults">The defaults instance to use, or null to clear.</param>
        public static void SetInstance(VoiceLiveAPIDefaults defaults)
        {
            explicitInstance = defaults;
        }

        /// <summary>
        ///     Clears the cached instance, forcing a reload on next access.
        /// </summary>
        public static void ClearCache()
        {
            instance = null;
            // Note: explicitInstance is intentionally NOT cleared
        }

        /// <summary>
        ///     Clears all instances including the explicit one.
        /// </summary>
        public static void ClearAll()
        {
            instance = null;
            explicitInstance = null;
        }

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            // When this asset is modified in the Inspector, clear the cache
            // so that changes are reflected immediately
#if UNITY_EDITOR
            // If this instance is being used, force a repaint of all inspectors
            if (instance == this || explicitInstance == this)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    // Repaint all inspector windows to reflect changes
                    var inspectors = Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
                    foreach (var inspector in inspectors)
                    {
                        if (inspector.GetType().Name == "InspectorWindow")
                        {
                            inspector.Repaint();
                        }
                    }
                };
            }
#endif
        }

        // Note: OnEnable is intentionally NOT used to auto-set instance
        // because it would cause every selected asset to become active.
        // Users should explicitly set the active defaults via the Inspector button.

        #endregion

        #region Avatar Settings

        [Header("Avatar Presets")]
        [Tooltip("Available avatar character:style combinations.\nFormat: Display Name|character:style")]
        [SerializeField]
        private string[] avatarPresets =
        {
            "Harry - Business|harry:business",
            "Harry - Casual|harry:casual",
            "Harry - Youthful|harry:youthful",
            "Jeff - Business|jeff:business",
            "Jeff - Formal|jeff:formal",
            "Lisa - Casual Sitting|lisa:casual-sitting",
            "Lisa - Casual Standing|lisa:casual-standing",
            "Lori - Casual|lori:casual",
            "Lori - Formal|lori:formal",
            "Lori - Graceful|lori:graceful",
            "Max - Business|max:business",
            "Max - Casual|max:casual",
            "Max - Formal|max:formal",
            "Meg - Business|meg:business",
            "Meg - Casual|meg:casual",
            "Meg - Formal|meg:formal"
        };

        [Header("Avatar Characters (Legacy)")]
        [Tooltip("Available avatar characters.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] characters =
        {
            "Harry|harry",
            "Jeff|jeff",
            "Lisa|lisa",
            "Lori|lori",
            "Max|max",
            "Meg|meg"
        };

        [Header("Avatar Styles (Legacy)")]
        [Tooltip("Available avatar styles.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] styles =
        {
            "Business|business",
            "Casual|casual",
            "Casual Sitting|casual-sitting",
            "Casual Standing|casual-standing",
            "Formal|formal",
            "Graceful|graceful",
            "Youthful|youthful"
        };

        #endregion

        #region Video Settings

        [Header("Video Codecs")]
        [Tooltip("Available video codecs.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] codecs =
        {
            "H.264|h264"
        };

        #endregion

        #region AI Model Settings

        [Header("AI Models")]
        [Tooltip("Available AI models.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] models =
        {
            "GPT Realtime|gpt-realtime",
            "GPT Realtime Mini|gpt-realtime-mini",
            "GPT-4o|gpt-4o",
            "GPT-4o Mini|gpt-4o-mini",
            "GPT-4.1|gpt-4.1",
            "GPT-4.1 Mini|gpt-4.1-mini",
            "GPT-5|gpt-5",
            "GPT-5 Mini|gpt-5-mini",
            "GPT-5 Nano|gpt-5-nano",
            "GPT-5 Chat|gpt-5-chat",
            "Phi4-mm Realtime|phi4-mm-realtime",
            "Phi4 Mini|phi4-mini"
        };

        #endregion

        #region Voice Settings

        [Header("Languages")]
        [Tooltip("Available languages for voice selection.\nFormat: Display Name|locale-code")]
        [SerializeField]
        private string[] languages =
        {
            "Afrikaans (South Africa)|af-ZA",
            "Amharic (Ethiopia)|am-ET",
            "Arabic (UAE)|ar-AE",
            "Arabic (Bahrain)|ar-BH",
            "Arabic (Algeria)|ar-DZ",
            "Arabic (Egypt)|ar-EG",
            "Arabic (Iraq)|ar-IQ",
            "Arabic (Jordan)|ar-JO",
            "Arabic (Kuwait)|ar-KW",
            "Arabic (Lebanon)|ar-LB",
            "Arabic (Libya)|ar-LY",
            "Arabic (Morocco)|ar-MA",
            "Arabic (Oman)|ar-OM",
            "Arabic (Qatar)|ar-QA",
            "Arabic (Saudi Arabia)|ar-SA",
            "Arabic (Syria)|ar-SY",
            "Arabic (Tunisia)|ar-TN",
            "Arabic (Yemen)|ar-YE",
            "Assamese (India)|as-IN",
            "Azerbaijani (Azerbaijan)|az-AZ",
            "Bulgarian (Bulgaria)|bg-BG",
            "Bengali (Bangladesh)|bn-BD",
            "Bengali (India)|bn-IN",
            "Bosnian (Bosnia)|bs-BA",
            "Catalan (Spain)|ca-ES",
            "Czech (Czech Republic)|cs-CZ",
            "Welsh (UK)|cy-GB",
            "Danish (Denmark)|da-DK",
            "German (Austria)|de-AT",
            "German (Switzerland)|de-CH",
            "German (Germany)|de-DE",
            "Greek (Greece)|el-GR",
            "English (Australia)|en-AU",
            "English (Canada)|en-CA",
            "English (UK)|en-GB",
            "English (Hong Kong)|en-HK",
            "English (Ireland)|en-IE",
            "English (India)|en-IN",
            "English (Kenya)|en-KE",
            "English (Nigeria)|en-NG",
            "English (New Zealand)|en-NZ",
            "English (Philippines)|en-PH",
            "English (Singapore)|en-SG",
            "English (Tanzania)|en-TZ",
            "English (US)|en-US",
            "English (South Africa)|en-ZA",
            "Spanish (Argentina)|es-AR",
            "Spanish (Bolivia)|es-BO",
            "Spanish (Chile)|es-CL",
            "Spanish (Colombia)|es-CO",
            "Spanish (Costa Rica)|es-CR",
            "Spanish (Cuba)|es-CU",
            "Spanish (Dominican Republic)|es-DO",
            "Spanish (Ecuador)|es-EC",
            "Spanish (Spain)|es-ES",
            "Spanish (Equatorial Guinea)|es-GQ",
            "Spanish (Guatemala)|es-GT",
            "Spanish (Honduras)|es-HN",
            "Spanish (Mexico)|es-MX",
            "Spanish (Nicaragua)|es-NI",
            "Spanish (Panama)|es-PA",
            "Spanish (Peru)|es-PE",
            "Spanish (Puerto Rico)|es-PR",
            "Spanish (Paraguay)|es-PY",
            "Spanish (El Salvador)|es-SV",
            "Spanish (US)|es-US",
            "Spanish (Uruguay)|es-UY",
            "Spanish (Venezuela)|es-VE",
            "Estonian (Estonia)|et-EE",
            "Basque (Spain)|eu-ES",
            "Persian (Iran)|fa-IR",
            "Finnish (Finland)|fi-FI",
            "Filipino (Philippines)|fil-PH",
            "French (Belgium)|fr-BE",
            "French (Canada)|fr-CA",
            "French (Switzerland)|fr-CH",
            "French (France)|fr-FR",
            "Irish (Ireland)|ga-IE",
            "Galician (Spain)|gl-ES",
            "Gujarati (India)|gu-IN",
            "Hebrew (Israel)|he-IL",
            "Hindi (India)|hi-IN",
            "Croatian (Croatia)|hr-HR",
            "Hungarian (Hungary)|hu-HU",
            "Armenian (Armenia)|hy-AM",
            "Indonesian (Indonesia)|id-ID",
            "Icelandic (Iceland)|is-IS",
            "Italian (Italy)|it-IT",
            "Inuktitut Syllabics (Canada)|iu-Cans-CA",
            "Inuktitut Latin (Canada)|iu-Latn-CA",
            "Japanese (Japan)|ja-JP",
            "Javanese (Indonesia)|jv-ID",
            "Georgian (Georgia)|ka-GE",
            "Kazakh (Kazakhstan)|kk-KZ",
            "Khmer (Cambodia)|km-KH",
            "Kannada (India)|kn-IN",
            "Korean (Korea)|ko-KR",
            "Lao (Laos)|lo-LA",
            "Lithuanian (Lithuania)|lt-LT",
            "Latvian (Latvia)|lv-LV",
            "Macedonian (North Macedonia)|mk-MK",
            "Malayalam (India)|ml-IN",
            "Mongolian (Mongolia)|mn-MN",
            "Marathi (India)|mr-IN",
            "Malay (Malaysia)|ms-MY",
            "Maltese (Malta)|mt-MT",
            "Burmese (Myanmar)|my-MM",
            "Norwegian Bokmal (Norway)|nb-NO",
            "Nepali (Nepal)|ne-NP",
            "Dutch (Belgium)|nl-BE",
            "Dutch (Netherlands)|nl-NL",
            "Odia (India)|or-IN",
            "Punjabi (India)|pa-IN",
            "Polish (Poland)|pl-PL",
            "Pashto (Afghanistan)|ps-AF",
            "Portuguese (Brazil)|pt-BR",
            "Portuguese (Portugal)|pt-PT",
            "Romanian (Romania)|ro-RO",
            "Russian (Russia)|ru-RU",
            "Sinhala (Sri Lanka)|si-LK",
            "Slovak (Slovakia)|sk-SK",
            "Slovenian (Slovenia)|sl-SI",
            "Somali (Somalia)|so-SO",
            "Albanian (Albania)|sq-AL",
            "Serbian Latin (Serbia)|sr-Latn-RS",
            "Serbian (Serbia)|sr-RS",
            "Sundanese (Indonesia)|su-ID",
            "Swedish (Sweden)|sv-SE",
            "Swahili (Kenya)|sw-KE",
            "Swahili (Tanzania)|sw-TZ",
            "Tamil (India)|ta-IN",
            "Tamil (Sri Lanka)|ta-LK",
            "Tamil (Malaysia)|ta-MY",
            "Tamil (Singapore)|ta-SG",
            "Telugu (India)|te-IN",
            "Thai (Thailand)|th-TH",
            "Turkish (Turkey)|tr-TR",
            "Ukrainian (Ukraine)|uk-UA",
            "Urdu (India)|ur-IN",
            "Urdu (Pakistan)|ur-PK",
            "Uzbek (Uzbekistan)|uz-UZ",
            "Vietnamese (Vietnam)|vi-VN",
            "Wu Chinese (China)|wuu-CN",
            "Cantonese (China)|yue-CN",
            "Chinese (China)|zh-CN",
            "Chinese Guangxi (China)|zh-CN-guangxi",
            "Chinese Henan (China)|zh-CN-henan",
            "Chinese Liaoning (China)|zh-CN-liaoning",
            "Chinese Shaanxi (China)|zh-CN-shaanxi",
            "Chinese Shandong (China)|zh-CN-shandong",
            "Chinese Sichuan (China)|zh-CN-sichuan",
            "Chinese (Hong Kong)|zh-HK",
            "Chinese (Taiwan)|zh-TW",
            "Zulu (South Africa)|zu-ZA"
        };

        [Header("Voice Names")]
        [Tooltip("Available voice names.\nFormat: Display Name|api-value\nNote: api-value should start with locale code (e.g., ja-JP, en-US)")]
        [SerializeField]
        private string[] voiceNames =
        {
            // Afrikaans (South Africa)
            "Adri|af-ZA-AdriNeural",
            "Willem|af-ZA-WillemNeural",

            // Amharic (Ethiopia)
            "Ameha|am-ET-AmehaNeural",
            "Mekdes|am-ET-MekdesNeural",

            // Arabic (UAE)
            "Fatima|ar-AE-FatimaNeural",
            "Hamdan|ar-AE-HamdanNeural",

            // Arabic (Bahrain)
            "Ali|ar-BH-AliNeural",
            "Laila|ar-BH-LailaNeural",

            // Arabic (Algeria)
            "Amina|ar-DZ-AminaNeural",
            "Ismael|ar-DZ-IsmaelNeural",

            // Arabic (Egypt)
            "Salma|ar-EG-SalmaNeural",
            "Shakir|ar-EG-ShakirNeural",

            // Arabic (Iraq)
            "Bassel|ar-IQ-BasselNeural",
            "Rana|ar-IQ-RanaNeural",

            // Arabic (Jordan)
            "Sana|ar-JO-SanaNeural",
            "Taim|ar-JO-TaimNeural",

            // Arabic (Kuwait)
            "Fahed|ar-KW-FahedNeural",
            "Noura|ar-KW-NouraNeural",

            // Arabic (Lebanon)
            "Layla|ar-LB-LaylaNeural",
            "Rami|ar-LB-RamiNeural",

            // Arabic (Libya)
            "Iman|ar-LY-ImanNeural",
            "Omar|ar-LY-OmarNeural",

            // Arabic (Morocco)
            "Jamal|ar-MA-JamalNeural",
            "Mouna|ar-MA-MounaNeural",

            // Arabic (Oman)
            "Abdullah|ar-OM-AbdullahNeural",
            "Aysha|ar-OM-AyshaNeural",

            // Arabic (Qatar)
            "Amal|ar-QA-AmalNeural",
            "Moaz|ar-QA-MoazNeural",

            // Arabic (Saudi Arabia)
            "Hamed|ar-SA-HamedNeural",
            "Zariyah|ar-SA-ZariyahNeural",

            // Arabic (Syria)
            "Amany|ar-SY-AmanyNeural",
            "Laith|ar-SY-LaithNeural",

            // Arabic (Tunisia)
            "Hedi|ar-TN-HediNeural",
            "Reem|ar-TN-ReemNeural",

            // Arabic (Yemen)
            "Maryam|ar-YE-MaryamNeural",
            "Saleh|ar-YE-SalehNeural",

            // Assamese (India)
            "Priyom|as-IN-PriyomNeural",
            "Yashica|as-IN-YashicaNeural",

            // Azerbaijani (Azerbaijan)
            "Babek|az-AZ-BabekNeural",
            "Banu|az-AZ-BanuNeural",

            // Bulgarian (Bulgaria)
            "Borislav|bg-BG-BorislavNeural",
            "Kalina|bg-BG-KalinaNeural",

            // Bengali (Bangladesh)
            "Nabanita|bn-BD-NabanitaNeural",
            "Pradeep|bn-BD-PradeepNeural",

            // Bengali (India)
            "Bashkar|bn-IN-BashkarNeural",
            "Tanishaa|bn-IN-TanishaaNeural",

            // Bosnian (Bosnia)
            "Goran|bs-BA-GoranNeural",
            "Vesna|bs-BA-VesnaNeural",

            // Catalan (Spain)
            "Alba|ca-ES-AlbaNeural",
            "Enric|ca-ES-EnricNeural",
            "Joana|ca-ES-JoanaNeural",

            // Czech (Czech Republic)
            "Antonin|cs-CZ-AntoninNeural",
            "Vlasta|cs-CZ-VlastaNeural",

            // Welsh (UK)
            "Aled|cy-GB-AledNeural",
            "Nia|cy-GB-NiaNeural",

            // Danish (Denmark)
            "Christel|da-DK-ChristelNeural",
            "Jeppe|da-DK-JeppeNeural",

            // German (Austria)
            "Ingrid|de-AT-IngridNeural",
            "Jonas|de-AT-JonasNeural",

            // German (Switzerland)
            "Jan|de-CH-JanNeural",
            "Leni|de-CH-LeniNeural",

            // German (Germany)
            "Amala|de-DE-AmalaNeural",
            "Bernd|de-DE-BerndNeural",
            "Christoph|de-DE-ChristophNeural",
            "Conrad|de-DE-ConradNeural",
            "Elke|de-DE-ElkeNeural",
            "Florian (HD)|de-DE-Florian:DragonHDLatestNeural",
            "Florian (Multilingual)|de-DE-FlorianMultilingualNeural",
            "Gisela|de-DE-GiselaNeural",
            "Kasper|de-DE-KasperNeural",
            "Katja|de-DE-KatjaNeural",
            "Killian|de-DE-KillianNeural",
            "Klarissa|de-DE-KlarissaNeural",
            "Klaus|de-DE-KlausNeural",
            "Louisa|de-DE-LouisaNeural",
            "Maja|de-DE-MajaNeural",
            "Ralf|de-DE-RalfNeural",
            "Seraphina (HD)|de-DE-Seraphina:DragonHDLatestNeural",
            "Seraphina (Multilingual)|de-DE-SeraphinaMultilingualNeural",
            "Tanja|de-DE-TanjaNeural",

            // Greek (Greece)
            "Athina|el-GR-AthinaNeural",
            "Nestoras|el-GR-NestorasNeural",

            // English (Australia)
            "Annette|en-AU-AnnetteNeural",
            "Carly|en-AU-CarlyNeural",
            "Darren|en-AU-DarrenNeural",
            "Duncan|en-AU-DuncanNeural",
            "Elsie|en-AU-ElsieNeural",
            "Freya|en-AU-FreyaNeural",
            "Joanne|en-AU-JoanneNeural",
            "Ken|en-AU-KenNeural",
            "Kim|en-AU-KimNeural",
            "Natasha|en-AU-NatashaNeural",
            "Neil|en-AU-NeilNeural",
            "Tim|en-AU-TimNeural",
            "Tina|en-AU-TinaNeural",
            "William (Multilingual)|en-AU-WilliamMultilingualNeural",
            "William|en-AU-WilliamNeural",

            // English (Canada)
            "Clara|en-CA-ClaraNeural",
            "Liam|en-CA-LiamNeural",

            // English (UK)
            "Abbi|en-GB-AbbiNeural",
            "Ada (Multilingual)|en-GB-AdaMultilingualNeural",
            "Alfie|en-GB-AlfieNeural",
            "Bella|en-GB-BellaNeural",
            "Elliot|en-GB-ElliotNeural",
            "Ethan|en-GB-EthanNeural",
            "Hollie|en-GB-HollieNeural",
            "Libby|en-GB-LibbyNeural",
            "Maisie|en-GB-MaisieNeural",
            "Noah|en-GB-NoahNeural",
            "Oliver|en-GB-OliverNeural",
            "Olivia|en-GB-OliviaNeural",
            "Ollie (Multilingual)|en-GB-OllieMultilingualNeural",
            "Ryan|en-GB-RyanNeural",
            "Sonia|en-GB-SoniaNeural",
            "Thomas|en-GB-ThomasNeural",

            // English (Hong Kong)
            "Sam|en-HK-SamNeural",
            "Yan|en-HK-YanNeural",

            // English (Ireland)
            "Connor|en-IE-ConnorNeural",
            "Emily|en-IE-EmilyNeural",

            // English (India)
            "Aarav|en-IN-AaravNeural",
            "Aarti (Indic)|en-IN-AartiIndicNeural",
            "Aarti|en-IN-AartiNeural",
            "Aashi|en-IN-AashiNeural",
            "Ananya|en-IN-AnanyaNeural",
            "Arjun (Indic)|en-IN-ArjunIndicNeural",
            "Arjun|en-IN-ArjunNeural",
            "Kavya|en-IN-KavyaNeural",
            "Kunal|en-IN-KunalNeural",
            "Neerja (Indic)|en-IN-NeerjaIndicNeural",
            "Neerja|en-IN-NeerjaNeural",
            "Prabhat (Indic)|en-IN-PrabhatIndicNeural",
            "Prabhat|en-IN-PrabhatNeural",
            "Rehaan|en-IN-RehaanNeural",

            // English (Kenya)
            "Asilia|en-KE-AsiliaNeural",
            "Chilemba|en-KE-ChilembaNeural",

            // English (Nigeria)
            "Abeo|en-NG-AbeoNeural",
            "Ezinne|en-NG-EzinneNeural",

            // English (New Zealand)
            "Mitchell|en-NZ-MitchellNeural",
            "Molly|en-NZ-MollyNeural",

            // English (Philippines)
            "James|en-PH-JamesNeural",
            "Rosa|en-PH-RosaNeural",

            // English (Singapore)
            "Luna|en-SG-LunaNeural",
            "Wayne|en-SG-WayneNeural",

            // English (Tanzania)
            "Elimu|en-TZ-ElimuNeural",
            "Imani|en-TZ-ImaniNeural",

            // English (US)
            "Adam (HD)|en-US-Adam:DragonHDLatestNeural",
            "Adam (Multilingual)|en-US-AdamMultilingualNeural",
            "AIGenerate1|en-US-AIGenerate1Neural",
            "AIGenerate2|en-US-AIGenerate2Neural",
            "Alloy (HD)|en-US-Alloy:DragonHDLatestNeural",
            "AlloyTurbo (Multilingual)|en-US-AlloyTurboMultilingualNeural",
            "Amanda (Multilingual)|en-US-AmandaMultilingualNeural",
            "Amber|en-US-AmberNeural",
            "Ana|en-US-AnaNeural",
            "Andrew (HD)|en-US-Andrew:DragonHDLatestNeural",
            "Andrew2 (HD)|en-US-Andrew2:DragonHDLatestNeural",
            "Andrew3 (HD)|en-US-Andrew3:DragonHDLatestNeural",
            "Andrew (Multilingual)|en-US-AndrewMultilingualNeural",
            "Andrew|en-US-AndrewNeural",
            "Aria (HD)|en-US-Aria:DragonHDLatestNeural",
            "Aria|en-US-AriaNeural",
            "Ashley|en-US-AshleyNeural",
            "AshTurbo (Multilingual)|en-US-AshTurboMultilingualNeural",
            "Ava (HD)|en-US-Ava:DragonHDLatestNeural",
            "Ava3 (HD)|en-US-Ava3:DragonHDLatestNeural",
            "Ava (Multilingual)|en-US-AvaMultilingualNeural",
            "Ava|en-US-AvaNeural",
            "Blue|en-US-BlueNeural",
            "Brandon (Multilingual)|en-US-BrandonMultilingualNeural",
            "Brandon|en-US-BrandonNeural",
            "Bree (HD)|en-US-Bree:DragonHDLatestNeural",
            "Brian (HD)|en-US-Brian:DragonHDLatestNeural",
            "Brian (Multilingual)|en-US-BrianMultilingualNeural",
            "Brian|en-US-BrianNeural",
            "Christopher (Multilingual)|en-US-ChristopherMultilingualNeural",
            "Christopher|en-US-ChristopherNeural",
            "Cora (Multilingual)|en-US-CoraMultilingualNeural",
            "Cora|en-US-CoraNeural",
            "Davis (HD)|en-US-Davis:DragonHDLatestNeural",
            "Davis (Multilingual)|en-US-DavisMultilingualNeural",
            "Davis|en-US-DavisNeural",
            "Derek (Multilingual)|en-US-DerekMultilingualNeural",
            "Dustin (Multilingual)|en-US-DustinMultilingualNeural",
            "EchoTurbo (Multilingual)|en-US-EchoTurboMultilingualNeural",
            "Elizabeth|en-US-ElizabethNeural",
            "Emma (HD)|en-US-Emma:DragonHDLatestNeural",
            "Emma2 (HD)|en-US-Emma2:DragonHDLatestNeural",
            "Emma (Multilingual)|en-US-EmmaMultilingualNeural",
            "Emma|en-US-EmmaNeural",
            "Eric|en-US-EricNeural",
            "Evelyn (Multilingual)|en-US-EvelynMultilingualNeural",
            "FableTurbo (Multilingual)|en-US-FableTurboMultilingualNeural",
            "Guy|en-US-GuyNeural",
            "Jacob|en-US-JacobNeural",
            "Jane (HD)|en-US-Jane:DragonHDLatestNeural",
            "Jane|en-US-JaneNeural",
            "Jason|en-US-JasonNeural",
            "Jenny (HD)|en-US-Jenny:DragonHDLatestNeural",
            "Jenny (Multilingual)|en-US-JennyMultilingualNeural",
            "Jenny|en-US-JennyNeural",
            "Kai|en-US-KaiNeural",
            "Lewis (Multilingual)|en-US-LewisMultilingualNeural",
            "Lola (Multilingual)|en-US-LolaMultilingualNeural",
            "Luna|en-US-LunaNeural",
            "Michelle|en-US-MichelleNeural",
            "Monica|en-US-MonicaNeural",
            "MultiTalker-Ava-Andrew (HD)|en-US-MultiTalker-Ava-Andrew:DragonHDLatestNeural",
            "MultiTalker-Ava-Steffan (HD)|en-US-MultiTalker-Ava-Steffan:DragonHDLatestNeural",
            "Nancy (Multilingual)|en-US-NancyMultilingualNeural",
            "Nancy|en-US-NancyNeural",
            "Nova (HD)|en-US-Nova:DragonHDLatestNeural",
            "NovaTurbo (Multilingual)|en-US-NovaTurboMultilingualNeural",
            "OnyxTurbo (Multilingual)|en-US-OnyxTurboMultilingualNeural",
            "Phoebe (HD)|en-US-Phoebe:DragonHDLatestNeural",
            "Phoebe (Multilingual)|en-US-PhoebeMultilingualNeural",
            "Roger|en-US-RogerNeural",
            "Ryan (Multilingual)|en-US-RyanMultilingualNeural",
            "Samuel (Multilingual)|en-US-SamuelMultilingualNeural",
            "Sara|en-US-SaraNeural",
            "Serena (HD)|en-US-Serena:DragonHDLatestNeural",
            "Serena (Multilingual)|en-US-SerenaMultilingualNeural",
            "ShimmerTurbo (Multilingual)|en-US-ShimmerTurboMultilingualNeural",
            "Steffan (HD)|en-US-Steffan:DragonHDLatestNeural",
            "Steffan (Multilingual)|en-US-SteffanMultilingualNeural",
            "Steffan|en-US-SteffanNeural",
            "Tony|en-US-TonyNeural",

            // English (South Africa)
            "Leah|en-ZA-LeahNeural",
            "Luke|en-ZA-LukeNeural",

            // Spanish (Argentina)
            "Elena|es-AR-ElenaNeural",
            "Tomas|es-AR-TomasNeural",

            // Spanish (Bolivia)
            "Marcelo|es-BO-MarceloNeural",
            "Sofia|es-BO-SofiaNeural",

            // Spanish (Chile)
            "Catalina|es-CL-CatalinaNeural",
            "Lorenzo|es-CL-LorenzoNeural",

            // Spanish (Colombia)
            "Gonzalo|es-CO-GonzaloNeural",
            "Salome|es-CO-SalomeNeural",

            // Spanish (Costa Rica)
            "Juan|es-CR-JuanNeural",
            "Maria|es-CR-MariaNeural",

            // Spanish (Cuba)
            "Belkys|es-CU-BelkysNeural",
            "Manuel|es-CU-ManuelNeural",

            // Spanish (Dominican Republic)
            "Emilio|es-DO-EmilioNeural",
            "Ramona|es-DO-RamonaNeural",

            // Spanish (Ecuador)
            "Andrea|es-EC-AndreaNeural",
            "Luis|es-EC-LuisNeural",

            // Spanish (Spain)
            "Abril|es-ES-AbrilNeural",
            "Alvaro|es-ES-AlvaroNeural",
            "Arabella (Multilingual)|es-ES-ArabellaMultilingualNeural",
            "Arnau|es-ES-ArnauNeural",
            "Dario|es-ES-DarioNeural",
            "Elias|es-ES-EliasNeural",
            "Elvira|es-ES-ElviraNeural",
            "Estrella|es-ES-EstrellaNeural",
            "Irene|es-ES-IreneNeural",
            "Isidora (Multilingual)|es-ES-IsidoraMultilingualNeural",
            "Laia|es-ES-LaiaNeural",
            "Lia|es-ES-LiaNeural",
            "Nil|es-ES-NilNeural",
            "Saul|es-ES-SaulNeural",
            "Teo|es-ES-TeoNeural",
            "Triana|es-ES-TrianaNeural",
            "Tristan (HD)|es-ES-Tristan:DragonHDLatestNeural",
            "Tristan (Multilingual)|es-ES-TristanMultilingualNeural",
            "Vera|es-ES-VeraNeural",
            "Ximena (HD)|es-ES-Ximena:DragonHDLatestNeural",
            "Ximena (Multilingual)|es-ES-XimenaMultilingualNeural",
            "Ximena|es-ES-XimenaNeural",

            // Spanish (Equatorial Guinea)
            "Javier|es-GQ-JavierNeural",
            "Teresa|es-GQ-TeresaNeural",

            // Spanish (Guatemala)
            "Andres|es-GT-AndresNeural",
            "Marta|es-GT-MartaNeural",

            // Spanish (Honduras)
            "Carlos|es-HN-CarlosNeural",
            "Karla|es-HN-KarlaNeural",

            // Spanish (Mexico)
            "Beatriz|es-MX-BeatrizNeural",
            "Candela|es-MX-CandelaNeural",
            "Carlota|es-MX-CarlotaNeural",
            "Cecilio|es-MX-CecilioNeural",
            "Dalia (Multilingual)|es-MX-DaliaMultilingualNeural",
            "Dalia|es-MX-DaliaNeural",
            "Gerardo|es-MX-GerardoNeural",
            "Jorge (Multilingual)|es-MX-JorgeMultilingualNeural",
            "Jorge|es-MX-JorgeNeural",
            "Larissa|es-MX-LarissaNeural",
            "Liberto|es-MX-LibertoNeural",
            "Luciano|es-MX-LucianoNeural",
            "Marina|es-MX-MarinaNeural",
            "Nuria|es-MX-NuriaNeural",
            "Pelayo|es-MX-PelayoNeural",
            "Renata|es-MX-RenataNeural",
            "Yago|es-MX-YagoNeural",

            // Spanish (Nicaragua)
            "Federico|es-NI-FedericoNeural",
            "Yolanda|es-NI-YolandaNeural",

            // Spanish (Panama)
            "Margarita|es-PA-MargaritaNeural",
            "Roberto|es-PA-RobertoNeural",

            // Spanish (Peru)
            "Alex|es-PE-AlexNeural",
            "Camila|es-PE-CamilaNeural",

            // Spanish (Puerto Rico)
            "Karina|es-PR-KarinaNeural",
            "Victor|es-PR-VictorNeural",

            // Spanish (Paraguay)
            "Mario|es-PY-MarioNeural",
            "Tania|es-PY-TaniaNeural",

            // Spanish (El Salvador)
            "Lorena|es-SV-LorenaNeural",
            "Rodrigo|es-SV-RodrigoNeural",

            // Spanish (US)
            "Alonso|es-US-AlonsoNeural",
            "Paloma|es-US-PalomaNeural",

            // Spanish (Uruguay)
            "Mateo|es-UY-MateoNeural",
            "Valentina|es-UY-ValentinaNeural",

            // Spanish (Venezuela)
            "Paola|es-VE-PaolaNeural",
            "Sebastian|es-VE-SebastianNeural",

            // Estonian (Estonia)
            "Anu|et-EE-AnuNeural",
            "Kert|et-EE-KertNeural",

            // Basque (Spain)
            "Ainhoa|eu-ES-AinhoaNeural",
            "Ander|eu-ES-AnderNeural",

            // Persian (Iran)
            "Dilara|fa-IR-DilaraNeural",
            "Farid|fa-IR-FaridNeural",

            // Finnish (Finland)
            "Harri|fi-FI-HarriNeural",
            "Noora|fi-FI-NooraNeural",
            "Selma|fi-FI-SelmaNeural",

            // Filipino (Philippines)
            "Angelo|fil-PH-AngeloNeural",
            "Blessica|fil-PH-BlessicaNeural",

            // French (Belgium)
            "Charline|fr-BE-CharlineNeural",
            "Gerard|fr-BE-GerardNeural",

            // French (Canada)
            "Antoine|fr-CA-AntoineNeural",
            "Jean|fr-CA-JeanNeural",
            "Sylvie|fr-CA-SylvieNeural",
            "Thierry|fr-CA-ThierryNeural",

            // French (Switzerland)
            "Ariane|fr-CH-ArianeNeural",
            "Fabrice|fr-CH-FabriceNeural",

            // French (France)
            "Alain|fr-FR-AlainNeural",
            "Brigitte|fr-FR-BrigitteNeural",
            "Celeste|fr-FR-CelesteNeural",
            "Claude|fr-FR-ClaudeNeural",
            "Coralie|fr-FR-CoralieNeural",
            "Denise|fr-FR-DeniseNeural",
            "Eloise|fr-FR-EloiseNeural",
            "Henri|fr-FR-HenriNeural",
            "Jacqueline|fr-FR-JacquelineNeural",
            "Jerome|fr-FR-JeromeNeural",
            "Josephine|fr-FR-JosephineNeural",
            "Lucien (Multilingual)|fr-FR-LucienMultilingualNeural",
            "Maurice|fr-FR-MauriceNeural",
            "Remy (HD)|fr-FR-Remy:DragonHDLatestNeural",
            "Remy (Multilingual)|fr-FR-RemyMultilingualNeural",
            "Vivienne (HD)|fr-FR-Vivienne:DragonHDLatestNeural",
            "Vivienne (Multilingual)|fr-FR-VivienneMultilingualNeural",
            "Yves|fr-FR-YvesNeural",
            "Yvette|fr-FR-YvetteNeural",

            // Irish (Ireland)
            "Colm|ga-IE-ColmNeural",
            "Orla|ga-IE-OrlaNeural",

            // Galician (Spain)
            "Roi|gl-ES-RoiNeural",
            "Sabela|gl-ES-SabelaNeural",

            // Gujarati (India)
            "Dhwani|gu-IN-DhwaniNeural",
            "Niranjan|gu-IN-NiranjanNeural",

            // Hebrew (Israel)
            "Avri|he-IL-AvriNeural",
            "Hila|he-IL-HilaNeural",

            // Hindi (India)
            "Aarav|hi-IN-AaravNeural",
            "Aarti|hi-IN-AartiNeural",
            "Ananya|hi-IN-AnanyaNeural",
            "Arjun|hi-IN-ArjunNeural",
            "Kavya|hi-IN-KavyaNeural",
            "Kunal|hi-IN-KunalNeural",
            "Madhur|hi-IN-MadhurNeural",
            "Rehaan|hi-IN-RehaanNeural",
            "Swara|hi-IN-SwaraNeural",

            // Croatian (Croatia)
            "Gabrijela|hr-HR-GabrijelaNeural",
            "Srecko|hr-HR-SreckoNeural",

            // Hungarian (Hungary)
            "Noemi|hu-HU-NoemiNeural",
            "Tamas|hu-HU-TamasNeural",

            // Armenian (Armenia)
            "Anahit|hy-AM-AnahitNeural",
            "Hayk|hy-AM-HaykNeural",

            // Indonesian (Indonesia)
            "Ardi|id-ID-ArdiNeural",
            "Gadis|id-ID-GadisNeural",

            // Icelandic (Iceland)
            "Gudrun|is-IS-GudrunNeural",
            "Gunnar|is-IS-GunnarNeural",

            // Italian (Italy)
            "Alessio (HD)|it-IT-Alessio:DragonHDLatestNeural",
            "Alessio (Multilingual)|it-IT-AlessioMultilingualNeural",
            "Benigno|it-IT-BenignoNeural",
            "Calimero|it-IT-CalimeroNeural",
            "Cataldo|it-IT-CataldoNeural",
            "Diego|it-IT-DiegoNeural",
            "Elsa|it-IT-ElsaNeural",
            "Fabiola|it-IT-FabiolaNeural",
            "Fiamma|it-IT-FiammaNeural",
            "Gianni|it-IT-GianniNeural",
            "Giuseppe (Multilingual)|it-IT-GiuseppeMultilingualNeural",
            "Giuseppe|it-IT-GiuseppeNeural",
            "Imelda|it-IT-ImeldaNeural",
            "Irma|it-IT-IrmaNeural",
            "Isabella (HD)|it-IT-Isabella:DragonHDLatestNeural",
            "Isabella (Multilingual)|it-IT-IsabellaMultilingualNeural",
            "Isabella|it-IT-IsabellaNeural",
            "Lisandro|it-IT-LisandroNeural",
            "Marcello (Multilingual)|it-IT-MarcelloMultilingualNeural",
            "Palmira|it-IT-PalmiraNeural",
            "Pierina|it-IT-PierinaNeural",
            "Rinaldo|it-IT-RinaldoNeural",

            // Inuktitut Syllabics (Canada)
            "Siqiniq|iu-Cans-CA-SiqiniqNeural",
            "Taqqiq|iu-Cans-CA-TaqqiqNeural",

            // Inuktitut Latin (Canada)
            "Siqiniq|iu-Latn-CA-SiqiniqNeural",
            "Taqqiq|iu-Latn-CA-TaqqiqNeural",

            // Japanese (Japan)
            "Aoi|ja-JP-AoiNeural",
            "Daichi|ja-JP-DaichiNeural",
            "Keita|ja-JP-KeitaNeural",
            "Masaru (HD)|ja-JP-Masaru:DragonHDLatestNeural",
            "Masaru (Multilingual)|ja-JP-MasaruMultilingualNeural",
            "Mayu|ja-JP-MayuNeural",
            "Nanami (HD)|ja-JP-Nanami:DragonHDLatestNeural",
            "Nanami|ja-JP-NanamiNeural",
            "Naoki|ja-JP-NaokiNeural",
            "Shiori|ja-JP-ShioriNeural",

            // Javanese (Indonesia)
            "Dimas|jv-ID-DimasNeural",
            "Siti|jv-ID-SitiNeural",

            // Georgian (Georgia)
            "Eka|ka-GE-EkaNeural",
            "Giorgi|ka-GE-GiorgiNeural",

            // Kazakh (Kazakhstan)
            "Aigul|kk-KZ-AigulNeural",
            "Daulet|kk-KZ-DauletNeural",

            // Khmer (Cambodia)
            "Piseth|km-KH-PisethNeural",
            "Sreymom|km-KH-SreymomNeural",

            // Kannada (India)
            "Gagan|kn-IN-GaganNeural",
            "Sapna|kn-IN-SapnaNeural",

            // Korean (Korea)
            "BongJin|ko-KR-BongJinNeural",
            "GookMin|ko-KR-GookMinNeural",
            "Hyunsu (Multilingual)|ko-KR-HyunsuMultilingualNeural",
            "Hyunsu|ko-KR-HyunsuNeural",
            "InJoon|ko-KR-InJoonNeural",
            "JiMin|ko-KR-JiMinNeural",
            "SeoHyeon|ko-KR-SeoHyeonNeural",
            "SoonBok|ko-KR-SoonBokNeural",
            "SunHi|ko-KR-SunHiNeural",
            "YuJin|ko-KR-YuJinNeural",

            // Lao (Laos)
            "Chanthavong|lo-LA-ChanthavongNeural",
            "Keomany|lo-LA-KeomanyNeural",

            // Lithuanian (Lithuania)
            "Leonas|lt-LT-LeonasNeural",
            "Ona|lt-LT-OnaNeural",

            // Latvian (Latvia)
            "Everita|lv-LV-EveritaNeural",
            "Nils|lv-LV-NilsNeural",

            // Macedonian (North Macedonia)
            "Aleksandar|mk-MK-AleksandarNeural",
            "Marija|mk-MK-MarijaNeural",

            // Malayalam (India)
            "Midhun|ml-IN-MidhunNeural",
            "Sobhana|ml-IN-SobhanaNeural",

            // Mongolian (Mongolia)
            "Bataa|mn-MN-BataaNeural",
            "Yesui|mn-MN-YesuiNeural",

            // Marathi (India)
            "Aarohi|mr-IN-AarohiNeural",
            "Manohar|mr-IN-ManoharNeural",

            // Malay (Malaysia)
            "Osman|ms-MY-OsmanNeural",
            "Yasmin|ms-MY-YasminNeural",

            // Maltese (Malta)
            "Grace|mt-MT-GraceNeural",
            "Joseph|mt-MT-JosephNeural",

            // Burmese (Myanmar)
            "Nilar|my-MM-NilarNeural",
            "Thiha|my-MM-ThihaNeural",

            // Norwegian Bokmal (Norway)
            "Finn|nb-NO-FinnNeural",
            "Iselin|nb-NO-IselinNeural",
            "Pernille|nb-NO-PernilleNeural",

            // Nepali (Nepal)
            "Hemkala|ne-NP-HemkalaNeural",
            "Sagar|ne-NP-SagarNeural",

            // Dutch (Belgium)
            "Arnaud|nl-BE-ArnaudNeural",
            "Dena|nl-BE-DenaNeural",

            // Dutch (Netherlands)
            "Colette|nl-NL-ColetteNeural",
            "Fenna|nl-NL-FennaNeural",
            "Maarten|nl-NL-MaartenNeural",

            // Odia (India)
            "Subhasini|or-IN-SubhasiniNeural",
            "Sukant|or-IN-SukantNeural",

            // Punjabi (India)
            "Ojas|pa-IN-OjasNeural",
            "Vaani|pa-IN-VaaniNeural",

            // Polish (Poland)
            "Agnieszka|pl-PL-AgnieszkaNeural",
            "Marek|pl-PL-MarekNeural",
            "Zofia|pl-PL-ZofiaNeural",

            // Pashto (Afghanistan)
            "GulNawaz|ps-AF-GulNawazNeural",
            "Latifa|ps-AF-LatifaNeural",

            // Portuguese (Brazil)
            "Antonio|pt-BR-AntonioNeural",
            "Brenda|pt-BR-BrendaNeural",
            "Donato|pt-BR-DonatoNeural",
            "Elza|pt-BR-ElzaNeural",
            "Fabio|pt-BR-FabioNeural",
            "Francisca|pt-BR-FranciscaNeural",
            "Giovanna|pt-BR-GiovannaNeural",
            "Humberto|pt-BR-HumbertoNeural",
            "Julio|pt-BR-JulioNeural",
            "Leila|pt-BR-LeilaNeural",
            "Leticia|pt-BR-LeticiaNeural",
            "Macerio (HD)|pt-BR-Macerio:DragonHDLatestNeural",
            "Macerio (Multilingual)|pt-BR-MacerioMultilingualNeural",
            "Manuela|pt-BR-ManuelaNeural",
            "Nicolau|pt-BR-NicolauNeural",
            "Thalita (HD)|pt-BR-Thalita:DragonHDLatestNeural",
            "Thalita (Multilingual)|pt-BR-ThalitaMultilingualNeural",
            "Thalita|pt-BR-ThalitaNeural",
            "Valerio|pt-BR-ValerioNeural",
            "Yara|pt-BR-YaraNeural",

            // Portuguese (Portugal)
            "Duarte|pt-PT-DuarteNeural",
            "Fernanda|pt-PT-FernandaNeural",
            "Raquel|pt-PT-RaquelNeural",

            // Romanian (Romania)
            "Alina|ro-RO-AlinaNeural",
            "Emil|ro-RO-EmilNeural",

            // Russian (Russia)
            "Dariya|ru-RU-DariyaNeural",
            "Dmitry|ru-RU-DmitryNeural",
            "Svetlana|ru-RU-SvetlanaNeural",

            // Sinhala (Sri Lanka)
            "Sameera|si-LK-SameeraNeural",
            "Thilini|si-LK-ThiliniNeural",

            // Slovak (Slovakia)
            "Lukas|sk-SK-LukasNeural",
            "Viktoria|sk-SK-ViktoriaNeural",

            // Slovenian (Slovenia)
            "Petra|sl-SI-PetraNeural",
            "Rok|sl-SI-RokNeural",

            // Somali (Somalia)
            "Muuse|so-SO-MuuseNeural",
            "Ubax|so-SO-UbaxNeural",

            // Albanian (Albania)
            "Anila|sq-AL-AnilaNeural",
            "Ilir|sq-AL-IlirNeural",

            // Serbian Latin (Serbia)
            "Nicholas|sr-Latn-RS-NicholasNeural",
            "Sophie|sr-Latn-RS-SophieNeural",

            // Serbian (Serbia)
            "Nicholas|sr-RS-NicholasNeural",
            "Sophie|sr-RS-SophieNeural",

            // Sundanese (Indonesia)
            "Jajang|su-ID-JajangNeural",
            "Tuti|su-ID-TutiNeural",

            // Swedish (Sweden)
            "Hillevi|sv-SE-HilleviNeural",
            "Mattias|sv-SE-MattiasNeural",
            "Sofie|sv-SE-SofieNeural",

            // Swahili (Kenya)
            "Rafiki|sw-KE-RafikiNeural",
            "Zuri|sw-KE-ZuriNeural",

            // Swahili (Tanzania)
            "Daudi|sw-TZ-DaudiNeural",
            "Rehema|sw-TZ-RehemaNeural",

            // Tamil (India)
            "Pallavi|ta-IN-PallaviNeural",
            "Valluvar|ta-IN-ValluvarNeural",

            // Tamil (Sri Lanka)
            "Kumar|ta-LK-KumarNeural",
            "Saranya|ta-LK-SaranyaNeural",

            // Tamil (Malaysia)
            "Kani|ta-MY-KaniNeural",
            "Surya|ta-MY-SuryaNeural",

            // Tamil (Singapore)
            "Anbu|ta-SG-AnbuNeural",
            "Venba|ta-SG-VenbaNeural",

            // Telugu (India)
            "Mohan|te-IN-MohanNeural",
            "Shruti|te-IN-ShrutiNeural",

            // Thai (Thailand)
            "Achara|th-TH-AcharaNeural",
            "Niwat|th-TH-NiwatNeural",
            "Premwadee|th-TH-PremwadeeNeural",

            // Turkish (Turkey)
            "Ahmet|tr-TR-AhmetNeural",
            "Emel|tr-TR-EmelNeural",

            // Ukrainian (Ukraine)
            "Ostap|uk-UA-OstapNeural",
            "Polina|uk-UA-PolinaNeural",

            // Urdu (India)
            "Gul|ur-IN-GulNeural",
            "Salman|ur-IN-SalmanNeural",

            // Urdu (Pakistan)
            "Asad|ur-PK-AsadNeural",
            "Uzma|ur-PK-UzmaNeural",

            // Uzbek (Uzbekistan)
            "Madina|uz-UZ-MadinaNeural",
            "Sardor|uz-UZ-SardorNeural",

            // Vietnamese (Vietnam)
            "HoaiMy|vi-VN-HoaiMyNeural",
            "NamMinh|vi-VN-NamMinhNeural",

            // Wu Chinese (China)
            "Xiaotong|wuu-CN-XiaotongNeural",
            "Yunzhe|wuu-CN-YunzheNeural",

            // Cantonese (China)
            "XiaoMin|yue-CN-XiaoMinNeural",
            "YunSong|yue-CN-YunSongNeural",

            // Chinese (China)
            "Xiaochen (HD Flash)|zh-CN-Xiaochen:DragonHDFlashLatestNeural",
            "Xiaochen (HD)|zh-CN-Xiaochen:DragonHDLatestNeural",
            "Xiaochen (Multilingual)|zh-CN-XiaochenMultilingualNeural",
            "Xiaochen|zh-CN-XiaochenNeural",
            "Xiaohan|zh-CN-XiaohanNeural",
            "Xiaomeng|zh-CN-XiaomengNeural",
            "Xiaomo|zh-CN-XiaomoNeural",
            "Xiaoqiu|zh-CN-XiaoqiuNeural",
            "Xiaorou|zh-CN-XiaorouNeural",
            "Xiaorui|zh-CN-XiaoruiNeural",
            "Xiaoshuang|zh-CN-XiaoshuangNeural",
            "Xiaoxiao (HD Flash)|zh-CN-Xiaoxiao:DragonHDFlashLatestNeural",
            "Xiaoxiao2 (HD Flash)|zh-CN-Xiaoxiao2:DragonHDFlashLatestNeural",
            "Xiaoxiao (Dialects)|zh-CN-XiaoxiaoDialectsNeural",
            "Xiaoxiao (Multilingual)|zh-CN-XiaoxiaoMultilingualNeural",
            "Xiaoxiao|zh-CN-XiaoxiaoNeural",
            "Xiaoyan|zh-CN-XiaoyanNeural",
            "Xiaoyi|zh-CN-XiaoyiNeural",
            "Xiaoyou|zh-CN-XiaoyouNeural",
            "Xiaoyu (Multilingual)|zh-CN-XiaoyuMultilingualNeural",
            "Xiaozhen|zh-CN-XiaozhenNeural",
            "Yunfan (HD)|zh-CN-Yunfan:DragonHDLatestNeural",
            "Yunfan (Multilingual)|zh-CN-YunfanMultilingualNeural",
            "Yunfeng|zh-CN-YunfengNeural",
            "Yunhao|zh-CN-YunhaoNeural",
            "Yunjian|zh-CN-YunjianNeural",
            "Yunjie|zh-CN-YunjieNeural",
            "Yunxia|zh-CN-YunxiaNeural",
            "Yunxiao (HD Flash)|zh-CN-Yunxiao:DragonHDFlashLatestNeural",
            "Yunxiao (Multilingual)|zh-CN-YunxiaoMultilingualNeural",
            "Yunxi|zh-CN-YunxiNeural",
            "Yunyang|zh-CN-YunyangNeural",
            "Yunye (HD Flash)|zh-CN-Yunye:DragonHDFlashLatestNeural",
            "Yunye|zh-CN-YunyeNeural",
            "Yunyi (HD Flash)|zh-CN-Yunyi:DragonHDFlashLatestNeural",
            "Yunyi (Multilingual)|zh-CN-YunyiMultilingualNeural",
            "Yunze|zh-CN-YunzeNeural",

            // Chinese Guangxi (China)
            "Yunqi|zh-CN-guangxi-YunqiNeural",

            // Chinese Henan (China)
            "Yundeng|zh-CN-henan-YundengNeural",

            // Chinese Liaoning (China)
            "Xiaobei|zh-CN-liaoning-XiaobeiNeural",
            "Yunbiao|zh-CN-liaoning-YunbiaoNeural",

            // Chinese Shaanxi (China)
            "Xiaoni|zh-CN-shaanxi-XiaoniNeural",

            // Chinese Shandong (China)
            "Yunxiang|zh-CN-shandong-YunxiangNeural",

            // Chinese Sichuan (China)
            "Yunxi|zh-CN-sichuan-YunxiNeural",

            // Chinese (Hong Kong)
            "HiuGaai|zh-HK-HiuGaaiNeural",
            "HiuMaan|zh-HK-HiuMaanNeural",
            "WanLung|zh-HK-WanLungNeural",

            // Chinese (Taiwan)
            "HsiaoChen|zh-TW-HsiaoChenNeural",
            "HsiaoYu|zh-TW-HsiaoYuNeural",
            "YunJhe|zh-TW-YunJheNeural",

            // Zulu (South Africa)
            "Thando|zu-ZA-ThandoNeural",
            "Themba|zu-ZA-ThembaNeural"
        };

        [Header("Voice Types")]
        [Tooltip("Available voice types.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] voiceTypes =
        {
            "Azure Standard|azure-standard",
            "Azure Custom|azure-custom"
        };

        #endregion

        #region Audio Settings

        [Header("Noise Reduction Types")]
        [Tooltip("Available noise reduction types.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] noiseReductionTypes =
        {
            "None|none",
            "Azure Deep Noise Suppression|azure_deep_noise_suppression",
            "Near Field(gpt-realtime,gpt-realtime-mini only)|near_field",
            "Far Field(gpt-realtime,gpt-realtime-mini only)|far_field"
        };

        #endregion

        #region Turn Detection Settings

        [Header("Turn Detection Types")]
        [Tooltip("Available turn detection types.\nFormat: Display Name|api-value")]
        [SerializeField]
        private string[] turnDetectionTypes =
        {
            "Server VAD|server_vad",
            "Azure Semantic VAD|azure_semantic_vad",
            "Azure Semantic VAD Multilingual|azure_semantic_vad_multilingual",
            "Semantic VAD(gpt-realtime,gpt-realtime-mini only)|semantic_vad"
        };

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the available languages.
        /// </summary>
        public string[] Languages => languages;

        /// <summary>
        ///     Gets the available avatar presets (character:style combinations).
        /// </summary>
        public string[] AvatarPresets => avatarPresets;

        /// <summary>
        ///     Gets the available avatar characters (legacy, use AvatarPresets instead).
        /// </summary>
        public string[] Characters => characters;

        /// <summary>
        ///     Gets the available avatar styles (legacy, use AvatarPresets instead).
        /// </summary>
        public string[] Styles => styles;

        /// <summary>
        ///     Gets the available video codecs.
        /// </summary>
        public string[] Codecs => codecs;

        /// <summary>
        ///     Gets the available AI models.
        /// </summary>
        public string[] Models => models;

        /// <summary>
        ///     Gets the available voice names.
        /// </summary>
        public string[] VoiceNames => voiceNames;

        /// <summary>
        ///     Gets the available voice types.
        /// </summary>
        public string[] VoiceTypes => voiceTypes;

        /// <summary>
        ///     Gets the available noise reduction types.
        /// </summary>
        public string[] NoiseReductionTypes => noiseReductionTypes;

        /// <summary>
        ///     Gets the available turn detection types.
        /// </summary>
        public string[] TurnDetectionTypes => turnDetectionTypes;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets options for a specific category.
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <returns>Array of options in "Display Name|api-value" format.</returns>
        public string[] GetOptions(string category)
        {
            return category?.ToLowerInvariant() switch
            {
                "languages" or "language" => languages,
                "avatarpresets" or "avatarpreset" => avatarPresets,
                "characters" or "character" => characters,
                "styles" or "style" => styles,
                "codecs" or "codec" => codecs,
                "models" or "model" => models,
                "voicenames" or "voicename" => voiceNames,
                "voicetypes" or "voicetype" => voiceTypes,
                "noisereductiontypes" or "noisereductiontype" or "noisereduction" => noiseReductionTypes,
                "turndetectiontypes" or "turndetectiontype" or "turndetection" => turnDetectionTypes,
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        ///     Gets voice names filtered by language code.
        /// </summary>
        /// <param name="languageCode">The language/locale code (e.g., "ja-JP", "en-US").</param>
        /// <returns>Array of voice options matching the language code.</returns>
        public string[] GetVoicesByLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode) || voiceNames == null)
            {
                return voiceNames ?? Array.Empty<string>();
            }

            var filtered = new System.Collections.Generic.List<string>();
            foreach (var voice in voiceNames)
            {
                ParseOption(voice, out _, out var apiValue);
                if (apiValue.StartsWith(languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(voice);
                }
            }

            return filtered.Count > 0 ? filtered.ToArray() : voiceNames;
        }

        /// <summary>
        ///     Parses an option string into display name and API value.
        /// </summary>
        /// <param name="option">The option string in "Display Name|api-value" format.</param>
        /// <param name="displayName">Output display name.</param>
        /// <param name="apiValue">Output API value.</param>
        public static void ParseOption(string option, out string displayName, out string apiValue)
        {
            if (string.IsNullOrEmpty(option))
            {
                displayName = string.Empty;
                apiValue = string.Empty;
                return;
            }

            var parts = option.Split('|');
            if (parts.Length >= 2)
            {
                displayName = parts[0];
                apiValue = parts[1];
            }
            else
            {
                displayName = option;
                apiValue = option;
            }
        }

        /// <summary>
        ///     Gets display names from options array.
        /// </summary>
        /// <param name="options">Options array.</param>
        /// <returns>Array of display names.</returns>
        public static string[] GetDisplayNames(string[] options)
        {
            if (options == null || options.Length == 0)
            {
                return Array.Empty<string>();
            }

            var names = new string[options.Length];
            for (var i = 0; i < options.Length; i++)
            {
                ParseOption(options[i], out names[i], out _);
            }

            return names;
        }

        /// <summary>
        ///     Gets API values from options array.
        /// </summary>
        /// <param name="options">Options array.</param>
        /// <returns>Array of API values.</returns>
        public static string[] GetApiValues(string[] options)
        {
            if (options == null || options.Length == 0)
            {
                return Array.Empty<string>();
            }

            var values = new string[options.Length];
            for (var i = 0; i < options.Length; i++)
            {
                ParseOption(options[i], out _, out values[i]);
            }

            return values;
        }

        /// <summary>
        ///     Finds the index of an API value in options array.
        /// </summary>
        /// <param name="options">Options array.</param>
        /// <param name="apiValue">API value to find.</param>
        /// <returns>Index of the value, or -1 if not found.</returns>
        public static int FindIndexByApiValue(string[] options, string apiValue)
        {
            if (options == null || string.IsNullOrEmpty(apiValue))
            {
                return -1;
            }

            for (var i = 0; i < options.Length; i++)
            {
                ParseOption(options[i], out _, out var value);
                if (string.Equals(value, apiValue, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
