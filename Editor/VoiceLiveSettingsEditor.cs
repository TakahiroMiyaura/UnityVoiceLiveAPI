// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

#if UNITY_EDITOR
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings;
using UnityEditor;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Editor
{
    /// <summary>
    ///     Handles initialization when the editor loads or scripts are recompiled.
    /// </summary>
    [InitializeOnLoad]
    internal static class VoiceLiveEditorInitializer
    {
        static VoiceLiveEditorInitializer()
        {
            // Clear the cache on domain reload to ensure fresh asset loading
            VoiceLiveAPIDefaults.ClearCache();
        }
    }

    /// <summary>
    ///     Editor utilities for VoiceLive API settings management.
    /// </summary>
    public static class VoiceLiveSettingsEditor
    {
        #region Menu Items - Connection Settings

        /// <summary>
        ///     Creates a new FoundryConnectionSettings asset.
        /// </summary>
        /// <remarks>
        ///     This asset contains sensitive information (API keys, tokens).
        ///     It is recommended to add this asset's path to .gitignore.
        /// </remarks>
        [MenuItem("Assets/Create/Microsoft Foundry/VoiceLive API/Foundry Connection Settings", false, 50)]
        public static void CreateFoundryConnectionSettings()
        {
            var settings = ScriptableObject.CreateInstance<FoundryConnectionSettings>();
            CreateAndSaveAsset(settings, "FoundryConnectionSettings");

            Debug.LogWarning(
                "[VoiceLive API] Created FoundryConnectionSettings asset.\n" +
                "IMPORTANT: This asset contains sensitive credentials.\n" +
                "Add the following to your .gitignore to prevent accidental commits:\n" +
                "  Assets/**/FoundryConnection*.asset");
        }

        #endregion

        #region Menu Items - Session Settings

        /// <summary>
        ///     Creates a new VoiceLiveSessionSettings asset with default Japanese settings.
        /// </summary>
        [MenuItem("Assets/Create/Microsoft Foundry/VoiceLive API/Session Settings (Japanese)", false, 100)]
        public static void CreateDefaultJapaneseSettings()
        {
            var settings = ScriptableObject.CreateInstance<VoiceLiveSessionSettings>();

            // Configure for Japanese
            settings.Instructions = "あなたは親切なAIアシスタントです。日本語で応答してください。";
            settings.Voice.Language = "ja-JP";
            settings.Voice.VoiceName = AzureVoiceNames.JaJPNanamiHD;

            CreateAndSaveAsset(settings, "VoiceLiveSettings_Japanese");
        }

        /// <summary>
        ///     Creates a new VoiceLiveSessionSettings asset with default English settings.
        /// </summary>
        [MenuItem("Assets/Create/Microsoft Foundry/VoiceLive API/Session Settings (English)", false, 101)]
        public static void CreateDefaultEnglishSettings()
        {
            var settings = ScriptableObject.CreateInstance<VoiceLiveSessionSettings>();

            // Configure for English
            settings.Instructions = "You are a helpful AI assistant. Please respond in English.";
            settings.Voice.Language = "en-US";
            settings.Voice.VoiceName = AzureVoiceNames.EnUSAvaHD;

            CreateAndSaveAsset(settings, "VoiceLiveSettings_English");
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates and saves a ScriptableObject asset.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject.</typeparam>
        /// <param name="settings">The settings object to save.</param>
        /// <param name="defaultName">The default file name.</param>
        private static void CreateAndSaveAsset<T>(T settings, string defaultName) where T : ScriptableObject
        {
            // Get the path from the current selection
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!AssetDatabase.IsValidFolder(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
            }

            // Create unique path
            var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{defaultName}.asset");

            // Create and save the asset
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the created asset
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);

            Debug.Log($"Created VoiceLiveSessionSettings at: {assetPath}");
        }

        #endregion
    }

    /// <summary>
    ///     Custom inspector for VoiceLiveSessionSettings.
    ///     Controls visibility of fields based on ConnectionMode.
    /// </summary>
    [CustomEditor(typeof(VoiceLiveSessionSettings))]
    public class VoiceLiveSessionSettingsEditor : UnityEditor.Editor
    {
        #region Private Fields

        private SerializedProperty connectionMode;
        private SerializedProperty model;
        private SerializedProperty instructions;
        private SerializedProperty voice;
        private SerializedProperty audioProcessing;
        private SerializedProperty turnDetection;
        private SerializedProperty avatarSettings;
        private SerializedProperty animation;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            connectionMode = serializedObject.FindProperty("connectionMode");
            model = serializedObject.FindProperty("model");
            instructions = serializedObject.FindProperty("instructions");
            voice = serializedObject.FindProperty("voice");
            audioProcessing = serializedObject.FindProperty("audioProcessing");
            turnDetection = serializedObject.FindProperty("turnDetection");
            avatarSettings = serializedObject.FindProperty("avatarSettings");
            animation = serializedObject.FindProperty("animation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Connection Mode (always visible)
            EditorGUILayout.LabelField("Connection Mode", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(connectionMode);
            EditorGUILayout.Space();

            var isAgentMode = (ConnectionMode)connectionMode.enumValueIndex == ConnectionMode.AIAgent;

            // AI Model Settings (only in AIModel mode)
            if (!isAgentMode)
            {
                EditorGUILayout.LabelField("AI Model Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(model, new GUIContent("Model"));
                EditorGUILayout.PropertyField(instructions);
                EditorGUILayout.Space();
            }
            else
            {
                // Show info box for Agent mode
                EditorGUILayout.HelpBox(
                    "AI Agent mode: Model and Instructions are managed by the Agent configuration in Microsoft Foundry.",
                    MessageType.Info);
                EditorGUILayout.Space();
            }

            // Voice Settings (always visible)
            EditorGUILayout.LabelField("Voice Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(voice, true);
            EditorGUILayout.Space();

            // Audio Processing (always visible)
            EditorGUILayout.LabelField("Audio Processing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(audioProcessing, true);
            EditorGUILayout.Space();

            // Turn Detection (always visible)
            EditorGUILayout.LabelField("Turn Detection (VAD)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(turnDetection, true);
            EditorGUILayout.Space();

            // Avatar Settings
            EditorGUILayout.LabelField("Avatar Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(avatarSettings, new GUIContent("Avatar Settings Asset"));
            if (avatarSettings.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "No avatar settings assigned. Avatar will be disabled for this session.\n" +
                    "Create via: Assets > Create > VoiceLive API > Avatar Settings",
                    MessageType.Info);
            }
            EditorGUILayout.Space();

            // Animation Settings (always visible)
            EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(animation, true);

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }

    /// <summary>
    ///     Custom inspector for VoiceLiveAPIDefaults.
    ///     Provides buttons to set this asset as the active defaults and refresh dropdowns.
    /// </summary>
    [CustomEditor(typeof(VoiceLiveAPIDefaults))]
    public class VoiceLiveAPIDefaultsEditor : UnityEditor.Editor
    {
        #region Unity Lifecycle

        public override void OnInspectorGUI()
        {
            var defaults = (VoiceLiveAPIDefaults)target;

            // Check if this asset is currently active
            // Note: We compare by reference, not by equality
            var currentInstance = VoiceLiveAPIDefaults.Instance;
            var isActive = ReferenceEquals(currentInstance, defaults);

            // Show status box
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Defaults Status", EditorStyles.boldLabel);

                if (isActive)
                {
                    var originalColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0.5f, 1f, 0.5f, 1f);
                    EditorGUILayout.HelpBox(
                        "✓ ACTIVE - All dropdowns use these options.",
                        MessageType.Info);
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "NOT ACTIVE - Click the button below to use these options for all dropdowns.",
                        MessageType.Warning);

                    // Show which asset is currently active
                    if (currentInstance != null)
                    {
                        EditorGUILayout.LabelField(
                            $"Currently active: {currentInstance.name}",
                            EditorStyles.miniLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            "Currently active: (none - using auto-discovered)",
                            EditorStyles.miniLabel);
                    }
                }

                EditorGUILayout.Space(5);

                // Buttons - always show both
                EditorGUILayout.BeginHorizontal();
                {
                    // Set as Active button
                    var buttonStyle = new GUIStyle(GUI.skin.button);
                    if (isActive)
                    {
                        GUI.enabled = false;
                        GUILayout.Button("✓ Currently Active", buttonStyle, GUILayout.Height(28));
                        GUI.enabled = true;
                    }
                    else
                    {
                        var originalBgColor = GUI.backgroundColor;
                        GUI.backgroundColor = new Color(0.3f, 0.7f, 1f, 1f);
                        if (GUILayout.Button("Set as Active Defaults", buttonStyle, GUILayout.Height(28)))
                        {
                            VoiceLiveAPIDefaults.SetInstance(defaults);
                            RefreshAllInspectors();
                            Debug.Log($"[VoiceLive API] Set '{defaults.name}' as active defaults.");
                        }
                        GUI.backgroundColor = originalBgColor;
                    }

                    // Refresh button
                    if (GUILayout.Button("Refresh Inspectors", GUILayout.Height(28), GUILayout.Width(120)))
                    {
                        VoiceLiveAPIDefaults.ClearCache();
                        RefreshAllInspectors();
                        Debug.Log("[VoiceLive API] Refreshed all inspectors.");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Draw default inspector for the arrays
            DrawDefaultInspector();
        }

        private static void RefreshAllInspectors()
        {
            // Repaint all inspector windows
            var inspectors = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var inspector in inspectors)
            {
                if (inspector.GetType().Name == "InspectorWindow")
                {
                    inspector.Repaint();
                }
            }

            // Also repaint any open project windows
            EditorApplication.RepaintProjectWindow();
        }

        #endregion
    }
}
#endif
