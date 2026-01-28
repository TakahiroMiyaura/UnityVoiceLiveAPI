// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     Settings for animation outputs in VoiceLive sessions.
    /// </summary>
    [Serializable]
    public class AnimationSettings
    {
        #region Fields

        [Header("Animation Outputs")]
        [Tooltip("Enable viseme ID output for lip sync")]
        [SerializeField]
        private bool enableVisemeId = true;

        [Tooltip("Enable blend shapes output")]
        [SerializeField]
        private bool enableBlendShapes;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets whether viseme ID output is enabled.
        /// </summary>
        public bool EnableVisemeId
        {
            get => enableVisemeId;
            set => enableVisemeId = value;
        }

        /// <summary>
        ///     Gets or sets whether blend shapes output is enabled.
        /// </summary>
        public bool EnableBlendShapes
        {
            get => enableBlendShapes;
            set => enableBlendShapes = value;
        }

        /// <summary>
        ///     Gets the list of enabled output types for API configuration.
        /// </summary>
        /// <returns>List of output type strings.</returns>
        public List<string> GetOutputTypes()
        {
            var outputs = new List<string>();

            if (enableVisemeId)
            {
                outputs.Add("viseme_id");
            }

            if (enableBlendShapes)
            {
                outputs.Add("blend_shapes");
            }

            return outputs;
        }

        #endregion
    }
}
