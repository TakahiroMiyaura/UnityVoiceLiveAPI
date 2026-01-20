// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Attribute to mark a string field as a dropdown populated from VoiceLiveAPIDefaults.
    /// </summary>
    /// <remarks>
    ///     Use this attribute on string fields to display a dropdown menu in the Inspector
    ///     with options loaded from VoiceLiveAPIDefaults ScriptableObject.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     [VoiceLiveDropdown("Characters")]
    ///     [SerializeField]
    ///     private string character = "lisa";
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field)]
    public class VoiceLiveDropdownAttribute : PropertyAttribute
    {
        #region Properties

        /// <summary>
        ///     Gets the category name to load options from VoiceLiveAPIDefaults.
        /// </summary>
        /// <remarks>
        ///     Valid categories: Characters, Styles, Codecs, Models, VoiceNames, VoiceTypes, NoiseReductionTypes
        /// </remarks>
        public string Category { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether to allow custom values not in the dropdown.
        /// </summary>
        public bool AllowCustom { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceLiveDropdownAttribute" /> class.
        /// </summary>
        /// <param name="category">
        ///     The category name to load options from.
        ///     Valid values: Characters, Styles, Codecs, Models, VoiceNames, VoiceTypes, NoiseReductionTypes
        /// </param>
        public VoiceLiveDropdownAttribute(string category)
        {
            Category = category;
            AllowCustom = true;
        }

        #endregion
    }
}
