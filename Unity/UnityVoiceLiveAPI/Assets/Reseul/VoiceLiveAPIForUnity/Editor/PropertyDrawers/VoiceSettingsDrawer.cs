// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

#if UNITY_EDITOR
using System;
using System.Linq;
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings;
using UnityEditor;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Editor.PropertyDrawers
{
    /// <summary>
    ///     Custom property drawer for VoiceSettings.
    ///     Displays a cascading dropdown where language selection filters available voices.
    /// </summary>
    [CustomPropertyDrawer(typeof(VoiceSettings))]
    public class VoiceSettingsDrawer : PropertyDrawer
    {
        #region Constants

        private const float Spacing = 2f;
        private const float HeaderSpacing = 8f;
        private const string CustomValueLabel = "Custom...";
        private const float DropdownWidth = 0.7f;
        private const float TextFieldWidth = 0.3f;

        // Field counts for height calculation
        // Voice Selection: header + language + voiceName + voiceType = 4
        // Voice Parameters: header + temperature + style + pitch + rate + volume = 6
        private const int TotalLines = 10;

        #endregion

        #region PropertyDrawer Implementation

        /// <summary>
        ///     Gets the height of the property.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var lineHeight = EditorGUIUtility.singleLineHeight;
            // Foldout + headers (2) with extra spacing + fields (8)
            return lineHeight + // foldout
                   HeaderSpacing + lineHeight + // Voice Selection header
                   (lineHeight + Spacing) * 3 + // language, voiceName, voiceType
                   HeaderSpacing + lineHeight + // Voice Parameters header
                   (lineHeight + Spacing) * 5 + // temperature, style, pitch, rate, volume
                   Spacing;
        }

        /// <summary>
        ///     Draws the property in the Inspector.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var lineHeight = EditorGUIUtility.singleLineHeight;

            // Draw foldout
            var foldoutRect = new Rect(position.x, position.y, position.width, lineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            // Increase indent for child properties
            EditorGUI.indentLevel++;

            var yPos = position.y + lineHeight + Spacing;
            var defaults = VoiceLiveAPIDefaults.Instance;

            // Get properties
            var languageProp = property.FindPropertyRelative("language");
            var voiceNameProp = property.FindPropertyRelative("voiceName");
            var voiceTypeProp = property.FindPropertyRelative("voiceType");
            var temperatureProp = property.FindPropertyRelative("temperature");
            var styleProp = property.FindPropertyRelative("style");
            var pitchProp = property.FindPropertyRelative("pitch");
            var rateProp = property.FindPropertyRelative("rate");
            var volumeProp = property.FindPropertyRelative("volume");

            // === Voice Selection Header ===
            yPos += HeaderSpacing;
            var headerRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.LabelField(headerRect, "Voice Selection", EditorStyles.boldLabel);
            yPos += lineHeight + Spacing;

            // 1. Language dropdown
            var fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            DrawLanguageDropdown(fieldRect, languageProp, defaults);
            yPos += lineHeight + Spacing;

            // 2. Voice Name dropdown (filtered by language)
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            DrawVoiceNameDropdown(fieldRect, voiceNameProp, languageProp.stringValue, defaults);
            yPos += lineHeight + Spacing;

            // 3. Voice Type dropdown
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            DrawStandardDropdown(fieldRect, voiceTypeProp, "Voice Type", "VoiceTypes", defaults);
            yPos += lineHeight + Spacing;

            // === Voice Parameters Header ===
            yPos += HeaderSpacing;
            headerRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.LabelField(headerRect, "Voice Parameters", EditorStyles.boldLabel);
            yPos += lineHeight + Spacing;

            // 4. Temperature slider
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, temperatureProp);
            yPos += lineHeight + Spacing;

            // 5. Style
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, styleProp);
            yPos += lineHeight + Spacing;

            // 6. Pitch
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, pitchProp);
            yPos += lineHeight + Spacing;

            // 7. Rate
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, rateProp);
            yPos += lineHeight + Spacing;

            // 8. Volume
            fieldRect = new Rect(position.x, yPos, position.width, lineHeight);
            EditorGUI.PropertyField(fieldRect, volumeProp);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Draws the language dropdown.
        /// </summary>
        private void DrawLanguageDropdown(Rect position, SerializedProperty property, VoiceLiveAPIDefaults defaults)
        {
            var options = defaults?.Languages ?? Array.Empty<string>();
            if (options.Length == 0)
            {
                EditorGUI.PropertyField(position, property, new GUIContent("Language"));
                return;
            }

            var displayNames = VoiceLiveAPIDefaults.GetDisplayNames(options);
            var apiValues = VoiceLiveAPIDefaults.GetApiValues(options);
            var currentIndex = VoiceLiveAPIDefaults.FindIndexByApiValue(options, property.stringValue);
            var selectedIndex = Mathf.Max(0, currentIndex);

            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUI.Popup(position, "Language", selectedIndex, displayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = apiValues[newIndex];
            }
        }

        /// <summary>
        ///     Draws the voice name dropdown filtered by language.
        /// </summary>
        private void DrawVoiceNameDropdown(Rect position, SerializedProperty property, string languageCode,
            VoiceLiveAPIDefaults defaults)
        {
            // Get filtered voices
            var options = defaults?.GetVoicesByLanguage(languageCode) ?? Array.Empty<string>();
            if (options.Length == 0)
            {
                EditorGUI.PropertyField(position, property, new GUIContent("Voice Name"));
                return;
            }

            var displayNames = VoiceLiveAPIDefaults.GetDisplayNames(options);
            var apiValues = VoiceLiveAPIDefaults.GetApiValues(options);
            var currentIndex = VoiceLiveAPIDefaults.FindIndexByApiValue(options, property.stringValue);

            // Add "Custom..." option
            var popupOptions = displayNames.Concat(new[] { CustomValueLabel }).ToArray();
            int selectedIndex;

            if (currentIndex >= 0)
            {
                selectedIndex = currentIndex;
            }
            else
            {
                // Value is not in list, select "Custom..."
                selectedIndex = popupOptions.Length - 1;
            }

            var isCustomMode = selectedIndex == popupOptions.Length - 1;

            if (isCustomMode)
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                var contentWidth = position.width - EditorGUIUtility.labelWidth;
                var contentX = position.x + EditorGUIUtility.labelWidth;

                var dropdownRect = new Rect(
                    contentX,
                    position.y,
                    contentWidth * DropdownWidth - Spacing,
                    position.height);
                var textFieldRect = new Rect(
                    contentX + contentWidth * DropdownWidth,
                    position.y,
                    contentWidth * TextFieldWidth,
                    position.height);

                EditorGUI.LabelField(labelRect, "Voice Name");

                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(dropdownRect, selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        property.stringValue = apiValues[newIndex];
                    }
                }

                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.TextField(textFieldRect, property.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = newValue;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(position, "Voice Name", selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        property.stringValue = apiValues[newIndex];
                    }
                    else
                    {
                        // "Custom..." selected, keep current value to enter custom mode
                        property.stringValue = "";
                    }
                }
            }
        }

        /// <summary>
        ///     Draws a standard dropdown with custom value support.
        /// </summary>
        private void DrawStandardDropdown(Rect position, SerializedProperty property, string label, string category,
            VoiceLiveAPIDefaults defaults)
        {
            var options = defaults?.GetOptions(category) ?? Array.Empty<string>();
            if (options.Length == 0)
            {
                EditorGUI.PropertyField(position, property, new GUIContent(label));
                return;
            }

            var displayNames = VoiceLiveAPIDefaults.GetDisplayNames(options);
            var apiValues = VoiceLiveAPIDefaults.GetApiValues(options);
            var currentIndex = VoiceLiveAPIDefaults.FindIndexByApiValue(options, property.stringValue);

            var popupOptions = displayNames.Concat(new[] { CustomValueLabel }).ToArray();
            int selectedIndex;

            if (currentIndex >= 0)
            {
                selectedIndex = currentIndex;
            }
            else
            {
                selectedIndex = popupOptions.Length - 1;
            }

            var isCustomMode = selectedIndex == popupOptions.Length - 1;

            if (isCustomMode)
            {
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                var contentWidth = position.width - EditorGUIUtility.labelWidth;
                var contentX = position.x + EditorGUIUtility.labelWidth;

                var dropdownRect = new Rect(
                    contentX,
                    position.y,
                    contentWidth * DropdownWidth - Spacing,
                    position.height);
                var textFieldRect = new Rect(
                    contentX + contentWidth * DropdownWidth,
                    position.y,
                    contentWidth * TextFieldWidth,
                    position.height);

                EditorGUI.LabelField(labelRect, label);

                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(dropdownRect, selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        property.stringValue = apiValues[newIndex];
                    }
                }

                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.TextField(textFieldRect, property.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = newValue;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(position, label, selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        property.stringValue = apiValues[newIndex];
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                }
            }
        }

        #endregion
    }
}
#endif
