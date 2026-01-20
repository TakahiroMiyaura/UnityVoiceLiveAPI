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
    ///     Custom property drawer for VoiceLiveDropdownAttribute.
    ///     Displays a dropdown menu populated from VoiceLiveAPIDefaults.
    /// </summary>
    [CustomPropertyDrawer(typeof(VoiceLiveDropdownAttribute))]
    public class VoiceLiveDropdownDrawer : PropertyDrawer
    {
        #region Constants

        private const string CustomValueLabel = "Custom...";
        private const float DropdownWidth = 0.7f;
        private const float TextFieldWidth = 0.3f;
        private const float Spacing = 2f;

        #endregion

        #region PropertyDrawer Implementation

        /// <summary>
        ///     Draws the property in the Inspector.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Only works with string properties
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var dropdownAttribute = (VoiceLiveDropdownAttribute)attribute;
            var defaults = VoiceLiveAPIDefaults.Instance;
            var options = defaults?.GetOptions(dropdownAttribute.Category) ?? Array.Empty<string>();

            if (options.Length == 0)
            {
                // No options available, show regular text field
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var contentRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                position.width - EditorGUIUtility.labelWidth,
                position.height);

            // Draw label
            EditorGUI.LabelField(labelRect, label);

            // Get current value and find index
            var currentValue = property.stringValue;
            var currentIndex = VoiceLiveAPIDefaults.FindIndexByApiValue(options, currentValue);

            // Build display names array with custom option
            var displayNames = VoiceLiveAPIDefaults.GetDisplayNames(options);
            var apiValues = VoiceLiveAPIDefaults.GetApiValues(options);

            string[] popupOptions;
            int selectedIndex;

            if (dropdownAttribute.AllowCustom)
            {
                // Add "Custom..." option at the end
                popupOptions = displayNames.Concat(new[] { CustomValueLabel }).ToArray();

                if (currentIndex >= 0)
                {
                    // Value is in the predefined list
                    selectedIndex = currentIndex;
                }
                else
                {
                    // Value is not in list (including empty), select "Custom..."
                    selectedIndex = popupOptions.Length - 1;
                }
            }
            else
            {
                popupOptions = displayNames;
                selectedIndex = Mathf.Max(0, currentIndex);
            }

            // Determine if we're in custom mode
            // Custom mode: when "Custom..." is selected (value not in predefined list)
            var isCustomMode = dropdownAttribute.AllowCustom &&
                               selectedIndex == popupOptions.Length - 1;

            if (isCustomMode)
            {
                // Show dropdown and text field side by side
                var dropdownRect = new Rect(
                    contentRect.x,
                    contentRect.y,
                    contentRect.width * DropdownWidth - Spacing,
                    contentRect.height);
                var textFieldRect = new Rect(
                    contentRect.x + contentRect.width * DropdownWidth,
                    contentRect.y,
                    contentRect.width * TextFieldWidth,
                    contentRect.height);

                // Draw dropdown
                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(dropdownRect, selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        // User selected a predefined value, switch to it
                        property.stringValue = apiValues[newIndex];
                    }
                    // If "Custom..." is re-selected, keep current value
                }

                // Draw text field for custom value
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.TextField(textFieldRect, currentValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = newValue;
                }
            }
            else
            {
                // Just show dropdown
                EditorGUI.BeginChangeCheck();
                var newIndex = EditorGUI.Popup(contentRect, selectedIndex, popupOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    if (newIndex < apiValues.Length)
                    {
                        // User selected a predefined value
                        property.stringValue = apiValues[newIndex];
                    }
                    else if (dropdownAttribute.AllowCustom && newIndex == popupOptions.Length - 1)
                    {
                        // User selected "Custom...", clear to enter custom mode
                        property.stringValue = "";
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        #endregion
    }
}
#endif
