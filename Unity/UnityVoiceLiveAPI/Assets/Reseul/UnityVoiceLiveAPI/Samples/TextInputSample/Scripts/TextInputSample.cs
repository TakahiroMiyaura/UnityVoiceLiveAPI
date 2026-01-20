// Copyright (c) 2026 Takahiro Miyaura
// Released under the Boost Software License 1.0
// https://opensource.org/license/bsl-1-0

using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Text input sample demonstrating text-based conversation with AI.
///     Allows users to type messages and receive AI responses via voice.
/// </summary>
public class TextInputSample : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Components")]
    [SerializeField]
    [Tooltip("TextMeshPro component for displaying conversation messages")]
    private TextMeshProUGUI messageArea;

    [SerializeField]
    [Tooltip("Input field for typing messages")]
    private TMP_InputField inputField;

    [SerializeField]
    [Tooltip("Button to send the typed message")]
    private Button sendButton;

    [SerializeField]
    [Tooltip("Text component on the send button")]
    private TextMeshProUGUI sendButtonText;

    [SerializeField]
    [Tooltip("Button to connect/disconnect")]
    private Button connectButton;

    [SerializeField]
    [Tooltip("Text component on the connect button")]
    private TextMeshProUGUI connectButtonText;

    [SerializeField]
    [Tooltip("Status text to show connection state")]
    private TextMeshProUGUI statusText;

    [SerializeField]
    [Tooltip("Toggle for enabling voice input alongside text")]
    private Toggle voiceInputToggle;

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

        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClicked);
            sendButton.interactable = false;
        }

        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnInputSubmit);
            inputField.interactable = false;
        }

        if (voiceInputToggle != null)
        {
            voiceInputToggle.onValueChanged.AddListener(OnVoiceInputToggleChanged);
            voiceInputToggle.interactable = false;
        }

        UpdateStatus("Ready to connect");
        UpdateConnectButtonText("Connect");
        UpdateSendButtonText("Send");
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
                EnableInputControls(true);
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
            EnableInputControls(false);
            StopVoiceInput();
        }
    }

    private void OnSendButtonClicked()
    {
        SendTextMessage();
    }

    private void OnInputSubmit(string text)
    {
        SendTextMessage();
    }

    private void OnVoiceInputToggleChanged(bool isOn)
    {
        if (!client.IsConnected)
        {
            return;
        }

        if (isOn)
        {
            client.StartRecording();
            isRecording = true;
            UpdateStatus("Voice input enabled");
        }
        else
        {
            client.StopRecording();
            isRecording = false;
            UpdateStatus("Voice input disabled");
        }
    }

    #endregion

    #region Event Handlers

    private void HandleSessionStarted()
    {
        Debug.Log("Session started!");
        AddMessage("[System] Session started. You can type or speak.");
        UpdateStatus("Session active");
    }

    private void HandleTranscript(string transcript)
    {
        AddMessage($"[You (voice)]: {transcript}");
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

    #region Private Methods

    private void SendTextMessage()
    {
        if (!client.IsConnected || inputField == null)
        {
            return;
        }

        var message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Display the sent message
        AddMessage($"[You (text)]: {message}");

        // Send message to AI
        client.SendTextMessage(message);

        // Clear input field
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void EnableInputControls(bool enabled)
    {
        if (sendButton != null)
        {
            sendButton.interactable = enabled;
        }

        if (inputField != null)
        {
            inputField.interactable = enabled;
            if (enabled)
            {
                inputField.ActivateInputField();
            }
        }

        if (voiceInputToggle != null)
        {
            voiceInputToggle.interactable = enabled;
        }
    }

    private void StopVoiceInput()
    {
        if (isRecording)
        {
            client.StopRecording();
            isRecording = false;
        }

        if (voiceInputToggle != null)
        {
            voiceInputToggle.isOn = false;
        }
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

    private void UpdateSendButtonText(string text)
    {
        if (sendButtonText != null)
        {
            sendButtonText.text = text;
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
