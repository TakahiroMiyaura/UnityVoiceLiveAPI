# Text Input Sample

[![日本語](https://img.shields.io/badge/lang-日本語-red.svg)](README_JP.md)

A sample demonstrating text-based conversation with AI, with optional voice input support.

## Features

- Text input field for typing messages
- Send button and Enter key support
- Optional voice input toggle
- Real-time transcript display (for voice input)
- AI response display with voice playback
- Connection status indicator

## Setup

### Scene Setup

1. Create a new scene or use an existing one
2. Create an empty GameObject named "VoiceLiveManager"
3. Add the following components to it:
   - `UnityVoiceLiveClient`
   - `TextInputSample`
   - `AudioSource` (for AI voice playback)

### UI Setup

Create the following UI elements and assign them in the Inspector:

| Field | UI Element | Description |
|:------|:-----------|:------------|
| Message Area | TextMeshProUGUI | Displays conversation messages |
| Input Field | TMP_InputField | Text input for typing messages |
| Send Button | Button | Sends the typed message |
| Send Button Text | TextMeshProUGUI | Text on send button |
| Connect Button | Button | Connects/disconnects from API |
| Connect Button Text | TextMeshProUGUI | Text on connect button |
| Status Text | TextMeshProUGUI | Shows current connection status |
| Voice Input Toggle | Toggle | Optional: enables voice input |

### Configuration

Configure the `UnityVoiceLiveClient` component:

1. Set **Endpoint** to your Microsoft Foundry endpoint URL
2. Set **Access Token** to your API key
3. Select **Connection Mode** (AIAgent or AIModel)
4. Assign the **Audio Source** component for playback

## Usage

### Text Input

1. Press **Connect** to establish connection
2. Wait for "Session active" status
3. Type your message in the input field
4. Press **Send** or hit Enter
5. AI response will play automatically

### Voice Input (Optional)

1. Enable the **Voice Input Toggle**
2. Speak your message
3. Toggle off to stop voice input
4. Text and voice messages can be mixed

## Code Overview

```csharp
// Connect to API
await client.Connect();

// Send text message
client.SendTextMessage("Hello, how are you?");

// Enable voice input
client.StartRecording();

// Disable voice input
client.StopRecording();
```

## Events

| Event | Description |
|:------|:------------|
| OnSessionStarted | Session is ready for conversation |
| OnTranscriptReceived | User's speech was transcribed (voice input) |
| OnResponseOutputItemDoneReceived | AI response text received |
| OnErrorOccurred | Error occurred during operation |

## Notes

- Text messages are displayed with `[You (text)]` prefix
- Voice transcripts are displayed with `[You (voice)]` prefix
- AI responses are displayed with `[AI]` prefix
- Both input methods can be used in the same session
