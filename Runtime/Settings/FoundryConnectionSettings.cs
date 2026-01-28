// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Core.Clients;
using UnityEngine;

namespace Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Settings
{
    /// <summary>
    ///     ScriptableObject containing Microsoft Foundry connection settings.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This asset contains sensitive information (API keys, tokens).
    ///         It is recommended to add this asset's path to .gitignore to prevent
    ///         accidental commits of credentials.
    ///     </para>
    ///     <para>
    ///         Example .gitignore entry:
    ///         <code>
    ///         # Microsoft Foundry connection settings (contains sensitive tokens)
    ///         Assets/**/FoundryConnection*.asset
    ///         </code>
    ///     </para>
    ///     <para>
    ///         Create instances of this asset via Assets > Create > VoiceLive API > Foundry Connection Settings.
    ///     </para>
    /// </remarks>
    public class FoundryConnectionSettings : ScriptableObject
    {
        #region Fields

        [Header("Microsoft Foundry Endpoint")]
        [Tooltip("Microsoft Foundry endpoint URL (e.g., https://your-endpoint.cognitiveservices.azure.com)")]
        [SerializeField]
        private string endpoint = "https://your-endpoint.cognitiveservices.azure.com";

        [Header("Authentication")]
        [Tooltip("Authentication type (ApiKey or BearerToken)")]
        [SerializeField]
        private AuthenticationType authenticationType = AuthenticationType.ApiKey;

        [Tooltip("API key or Bearer token for authentication.\n" +
                 "WARNING: This is sensitive information. Add this asset to .gitignore.")]
        [SerializeField]
        private string accessToken = "";

        [Header("Agent Settings")]
        [Tooltip("Microsoft Foundry project name")]
        [SerializeField]
        private string projectName = "";

        [Tooltip("Agent ID")]
        [SerializeField]
        private string agentId = "";

        [Tooltip("Agent access token (optional, defaults to accessToken if empty).\n" +
                 "WARNING: This is sensitive information. Add this asset to .gitignore.")]
        [SerializeField]
        private string agentAccessToken = "";

        [Header("API Settings")]
        [Tooltip("API version")]
        [SerializeField]
        private string apiVersion = "2025-10-01";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Microsoft Foundry endpoint URL.
        /// </summary>
        public string Endpoint
        {
            get => endpoint;
            set => endpoint = value;
        }

        /// <summary>
        ///     Gets or sets the authentication type.
        /// </summary>
        public AuthenticationType AuthenticationType
        {
            get => authenticationType;
            set => authenticationType = value;
        }

        /// <summary>
        ///     Gets or sets the access token (API key or Bearer token).
        /// </summary>
        public string AccessToken
        {
            get => accessToken;
            set => accessToken = value;
        }

        /// <summary>
        ///     Gets or sets the project name.
        /// </summary>
        public string ProjectName
        {
            get => projectName;
            set => projectName = value;
        }

        /// <summary>
        ///     Gets or sets the agent ID.
        /// </summary>
        public string AgentId
        {
            get => agentId;
            set => agentId = value;
        }

        /// <summary>
        ///     Gets or sets the agent access token.
        /// </summary>
        public string AgentAccessToken
        {
            get => agentAccessToken;
            set => agentAccessToken = value;
        }

        /// <summary>
        ///     Gets the effective agent access token (uses accessToken if agentAccessToken is empty).
        /// </summary>
        public string EffectiveAgentAccessToken =>
            string.IsNullOrEmpty(agentAccessToken) ? accessToken : agentAccessToken;

        /// <summary>
        ///     Gets or sets the API version.
        /// </summary>
        public string ApiVersion
        {
            get => apiVersion;
            set => apiVersion = value;
        }

        /// <summary>
        ///     Gets a value indicating whether the connection settings are valid.
        /// </summary>
        public bool IsValid =>
            !string.IsNullOrEmpty(endpoint) &&
            !string.IsNullOrEmpty(accessToken) &&
            !string.IsNullOrEmpty(projectName) &&
            !string.IsNullOrEmpty(agentId);

        #endregion
    }
}
