# Avatar Sample

[![日本語](https://img.shields.io/badge/lang-日本語-red.svg)](README_JP.md)

A sample demonstrating Azure AI Avatar integration with UnityVoiceLiveClient using WebRTC for real-time video and audio streaming.

## Features

- Automatic connection on scene start
- AI Avatar video streaming via WebRTC
- Real-time lip-sync and avatar animation
- Voice input with live transcription
- Avatar audio response playback
- Message display area

## Prerequisites

This sample requires the **Unity WebRTC** package to be installed:

1. Open **Window > Package Manager**
2. Click **+** and select **Add package from git URL...**
3. Enter: `com.unity.webrtc`
4. Click **Add**

## Setup

### Scene Setup

1. Create a new scene or use the provided `Avatar.unity`
2. Create an empty GameObject named "VoiceLiveManager"
3. Add the following components to it:
   - `UnityVoiceLiveClient`
   - `UnityAvatarClient`
   - `AgentSample`
   - `AudioSource` (for avatar audio playback)

### UI Setup

Create the following UI elements and assign them in the Inspector:

#### AgentSample Component

| Field | UI Element | Description |
|:------|:-----------|:------------|
| Message Area | TextMeshProUGUI | Displays conversation messages |

#### UnityAvatarClient Component

| Field | UI Element | Description |
|:------|:-----------|:------------|
| Video Display | RawImage | Displays avatar video stream |
| Audio Source | AudioSource | Plays avatar audio stream |

### Configuration

#### UnityVoiceLiveClient Component

1. Set **Endpoint** to your Microsoft Foundry endpoint URL
2. Set **Access Token** to your API key
3. Select **Connection Mode** (AIAgent or AIModel)
4. Assign the **Audio Source** component for playback
5. **Important**: Assign **Avatar Settings** asset to enable avatar mode

#### Avatar Settings

Create or use an existing `VoiceLiveAvatarSettings` asset:

1. Right-click in Project window
2. Select **Create > Microsoft Foundry > VoiceLive API > Avatar Settings**
3. Configure the following:

| Setting | Description |
|:--------|:------------|
| Avatar Preset | Character and style (e.g., "lisa:casual-sitting") |
| Video Settings | Resolution and format options |
| Background Settings | Avatar background configuration |

## Usage

1. Configure the Microsoft Foundry connection settings
2. Enter Play mode
3. The sample automatically connects to the API
4. Wait for "Session started" message
5. Start speaking - your voice will be transcribed
6. The AI Avatar will respond with synchronized lip movements
7. Conversation history appears in the message area

## Code Overview

```csharp
// AgentSample auto-connects on Start
private async void Start()
{
    client = GetComponent<UnityVoiceLiveClient>();

    // Setup event listeners
    client.OnSessionStarted.AddListener(HandleSessionStarted);
    client.OnTranscriptReceived.AddListener(HandleTranscript);
    client.OnResponseOutputItemDoneReceived.AddListener(AddMessage);
    client.OnErrorOccurred.AddListener(HandleError);

    // Connect to Azure AI VoiceLive API
    await client.Connect();
}
```

## Events

### AgentSample Events

| Event | Description |
|:------|:------------|
| OnSessionStarted | Session is ready for conversation |
| OnTranscriptReceived | User's speech was transcribed |
| OnResponseOutputItemDoneReceived | AI response text received |
| OnErrorOccurred | Error occurred during operation |

### UnityAvatarClient Events

| Event | Description |
|:------|:------------|
| OnConnectionEstablished | WebRTC connection established |
| OnConnectionClosed | WebRTC connection closed |
| OnVideoFrameReceived | Video frame received from avatar |
| OnAudioFrameReceived | Audio frame received from avatar |
| OnError | Error occurred in avatar client |

## Available Avatar Presets

Common avatar character:style combinations include:

- `lisa:casual-sitting`
- `lisa:graceful-sitting`
- `lisa:graceful-standing`
- `harry:casual-sitting`
- `harry:graceful-sitting`
- `harry:graceful-standing`

For a complete list of available presets, refer to the Azure AI Avatar documentation.

## Troubleshooting

### Avatar video not displaying
- Ensure Unity WebRTC package is installed
- Check that RawImage is assigned to Video Display
- Verify Avatar Settings asset is assigned to UnityVoiceLiveClient

### Audio not playing
- Ensure AudioSource is assigned to UnityAvatarClient
- Check AudioSource volume and mute settings

### Connection fails
- Verify Microsoft Foundry endpoint and access token
- Ensure avatar feature is enabled in your Foundry configuration
