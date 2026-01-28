// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components;
using TMPro;
using UnityEngine;

/// <summary>
///     Sample script demonstrating basic usage of UnityVoiceLiveClient.
///     Connects to Azure AI VoiceLive API and displays received messages.
/// </summary>
public class AgentSample : MonoBehaviour
{
    [SerializeField]
    [Tooltip("TextMeshPro UI component for displaying messages")]
    private TextMeshProUGUI messageArea;

    private UnityVoiceLiveClient client;

    private async void Start()
    {
        client = GetComponent<UnityVoiceLiveClient>();

        // Setup event listeners
        client.OnSessionStarted.AddListener(HandleSessionStarted);
        client.OnTranscriptReceived.AddListener(HandleTranscript);
        client.OnResponseOutputItemDoneReceived.AddListener(AddMessage);
        client.OnErrorOccurred.AddListener(HandleError);

        // Connect to Azure AI VoiceLive API
        var connected = await client.Connect();
        if (!connected)
        {
            Debug.LogError("Failed to connect to Azure AI VoiceLive API");
        }
    }

    private void HandleSessionStarted()
    {
        Debug.Log("Agent session started!");
        AddMessage("[System] Session started. You can now speak.");
    }

    private void HandleTranscript(string transcript)
    {
        // Format the user transcript with a prefix
        AddMessage($"[user]: {transcript}");
    }

    private void HandleError(string error)
    {
        Debug.LogError($"Error: {error}");
        AddMessage($"[Error] {error}");
    }

    /// <summary>
    ///     Adds a message to the message area display.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void AddMessage(string message)
    {
        if (messageArea != null)
        {
            messageArea.text += message + "\n";
        }
    }
}
