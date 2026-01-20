# Basic Voice Chat Sample

[![日本語](https://img.shields.io/badge/lang-日本語-red.svg)](README_JP.md)

A simple voice chat sample demonstrating basic usage of UnityVoiceLiveClient without avatar features.

## Features

- Connect/Disconnect button
- Start/Stop recording button
- Real-time transcript display
- AI response display
- Connection status indicator

## Setup

### Scene Setup

1. Create a new scene or use an existing one
2. Create an empty GameObject named "VoiceLiveManager"
3. Add the following components to it:
   - `UnityVoiceLiveClient`
   - `BasicVoiceChatSample`
   - `AudioSource` (for AI voice playback)

### UI Setup

Create the following UI elements and assign them in the Inspector:

| Field | UI Element | Description |
|:------|:-----------|:------------|
| Message Area | TextMeshProUGUI | Displays conversation messages |
| Record Button | Button | Starts/stops voice recording |
| Record Button Text | TextMeshProUGUI | Text on record button |
| Connect Button | Button | Connects/disconnects from API |
| Connect Button Text | TextMeshProUGUI | Text on connect button |
| Status Text | TextMeshProUGUI | Shows current connection status |

### Configuration

Configure the `UnityVoiceLiveClient` component:

1. Set **Endpoint** to your Microsoft Foundry endpoint URL
2. Set **Access Token** to your API key
3. Select **Connection Mode** (AIAgent or AIModel)
4. Assign the **Audio Source** component for playback

## Usage

1. Press **Connect** to establish connection
2. Wait for "Session active" status
3. Press **Start Recording** to begin speaking
4. Speak your message
5. Press **Stop Recording** when finished
6. AI response will play automatically
7. Press **Disconnect** when done

## Code Overview

```csharp
// Connect to API
await client.Connect();

// Start voice recording
client.StartRecording();

// Stop voice recording
client.StopRecording();

// Disconnect
client.Disconnect();
```

## Events

| Event | Description |
|:------|:------------|
| OnSessionStarted | Session is ready for conversation |
| OnTranscriptReceived | User's speech was transcribed |
| OnResponseOutputItemDoneReceived | AI response text received |
| OnErrorOccurred | Error occurred during operation |
