// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Basic voice chat sample without avatar.
///     Demonstrates simple voice conversation with AI using UnityVoiceLiveClient.
/// </summary>
public class BasicVoiceChatSample : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Components")]
    [SerializeField]
    [Tooltip("TextMeshPro component for displaying conversation messages")]
    private TextMeshProUGUI messageArea;

    [SerializeField]
    [Tooltip("Button to start/stop recording")]
    private Button recordButton;

    [SerializeField]
    [Tooltip("Text component on the record button")]
    private TextMeshProUGUI recordButtonText;

    [SerializeField]
    [Tooltip("Button to connect/disconnect")]
    private Button connectButton;

    [SerializeField]
    [Tooltip("Text component on the connect button")]
    private TextMeshProUGUI connectButtonText;

    [SerializeField]
    [Tooltip("Status text to show connection state")]
    private TextMeshProUGUI statusText;

    #endregion

    #region Private Fields

    private UnityVoiceLiveClient client;
    private bool isRecording;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        client = GetComponent<UnityVoiceLiveClient>();

        // Setup event listeners
        client.OnSessionStarted.AddListener(HandleSessionStarted);
        client.OnTranscriptReceived.AddListener(HandleTranscript);
        client.OnResponseOutputItemDoneReceived.AddListener(HandleResponse);
        client.OnErrorOccurred.AddListener(HandleError);

        // Setup button listeners
        if (connectButton != null)
        {
            connectButton.onClick.AddListener(OnConnectButtonClicked);
        }

        if (recordButton != null)
        {
            recordButton.onClick.AddListener(OnRecordButtonClicked);
            recordButton.interactable = false; // Disable until connected
        }

        UpdateStatus("Ready to connect");
        UpdateConnectButtonText("Connect");
        UpdateRecordButtonText("Start Recording");
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        if (client != null)
        {
            client.OnSessionStarted.RemoveListener(HandleSessionStarted);
            client.OnTranscriptReceived.RemoveListener(HandleTranscript);
            client.OnResponseOutputItemDoneReceived.RemoveListener(HandleResponse);
            client.OnErrorOccurred.RemoveListener(HandleError);
        }
    }

    #endregion

    #region Button Handlers

    private async void OnConnectButtonClicked()
    {
        if (!client.IsConnected)
        {
            UpdateStatus("Connecting...");
            connectButton.interactable = false;

            var success = await client.Connect();

            if (success)
            {
                UpdateStatus("Connected");
                UpdateConnectButtonText("Disconnect");
                if (recordButton != null)
                {
                    recordButton.interactable = true;
                }
            }
            else
            {
                UpdateStatus("Connection failed");
            }

            connectButton.interactable = true;
        }
        else
        {
            client.Disconnect();
            UpdateStatus("Disconnected");
            UpdateConnectButtonText("Connect");
            if (recordButton != null)
            {
                recordButton.interactable = false;
            }
            isRecording = false;
            UpdateRecordButtonText("Start Recording");
        }
    }

    private void OnRecordButtonClicked()
    {
        if (!client.IsConnected)
        {
            return;
        }

        if (!isRecording)
        {
            client.StartRecording();
            isRecording = true;
            UpdateRecordButtonText("Stop Recording");
            UpdateStatus("Recording...");
        }
        else
        {
            client.StopRecording();
            isRecording = false;
            UpdateRecordButtonText("Start Recording");
            UpdateStatus("Connected");
        }
    }

    #endregion

    #region Event Handlers

    private void HandleSessionStarted()
    {
        Debug.Log("Session started!");
        AddMessage("[System] Session started. You can now speak.");
        UpdateStatus("Session active");
    }

    private void HandleTranscript(string transcript)
    {
        AddMessage($"[You]: {transcript}");
    }

    private void HandleResponse(string response)
    {
        AddMessage($"[AI]: {response}");
    }

    private void HandleError(string error)
    {
        Debug.LogError($"Error: {error}");
        AddMessage($"[Error] {error}");
        UpdateStatus($"Error: {error}");
    }

    #endregion

    #region UI Helpers

    private void AddMessage(string message)
    {
        if (messageArea != null)
        {
            messageArea.text += message + "\n";
        }
    }

    private void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }

    private void UpdateConnectButtonText(string text)
    {
        if (connectButtonText != null)
        {
            connectButtonText.text = text;
        }
    }

    private void UpdateRecordButtonText(string text)
    {
        if (recordButtonText != null)
        {
            recordButtonText.text = text;
        }
    }

    /// <summary>
    ///     Clears all messages from the message area.
    /// </summary>
    public void ClearMessages()
    {
        if (messageArea != null)
        {
            messageArea.text = "";
        }
    }

    #endregion
}
