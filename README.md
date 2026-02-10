# VoiceLive API for Unity

[![日本語](https://img.shields.io/badge/lang-日本語-red.svg)](README_JP.md)

Unity integration library for Azure AI VoiceLive API. Enables real-time voice conversations with AI models (GPT-4o, etc.) and custom AI agents in Unity applications.

## Overview

This library provides a wrapper to easily use the [Microsoft Foundry](https://azure.microsoft.com/products/ai-foundry) VoiceLive API from Unity. It abstracts complex processes such as WebSocket communication, audio capture/playback, and avatar video streaming, providing them as Inspector-configurable components.

## Features

- **Real-time Voice Conversations**: Voice interactions with Azure AI models (GPT-4o) and custom agents
- **Unity Components**: MonoBehaviour-based components configurable via Inspector
- **WebSocket Integration**: Efficient WebSocket communication with main thread event handling
- **Audio Processing**: Microphone input capture and AudioSource playback (PCM16 format)
- **Avatar Support**: Avatar video streaming via Unity WebRTC (optional)

## Requirements

- Unity 6000.0 or later
- .NET Standard 2.1
- Microsoft Foundry account with VoiceLive API access

### Optional Dependencies

- `com.unity.webrtc` 3.0.0 or later (required for avatar features)

## Installation

### Via Unity Package Manager

1. Open Unity Package Manager (Window → Package Manager)
2. Click "+" button → "Add package from git URL"
3. Enter one of the following URLs:

**Latest version (upm branch):**
```
https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI.git#upm
```

**Specific version (e.g., 1.0.0):**
```
https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI.git#upm@1.0.0
```

### Manual Installation

1. Clone or download this repository
2. Copy the `Unity/UnityVoiceLiveAPI/Assets/Reseul/UnityVoiceLiveAPI` folder to your Unity project's `Packages` directory
3. Ensure required dependencies are installed:
   - `com.unity.nuget.newtonsoft-json` (3.2.1 or later)

## Quick Start

### Basic Usage

1. Create an empty GameObject in your scene
2. Attach the `UnityVoiceLiveClient` component
3. Configure connection settings in the Inspector:
   - Endpoint: Azure AI endpoint URL
   - Access Token: API key or Bearer token
   - Connection Mode: Select AIAgent or AIModel

### Code Example

```csharp
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components;
using UnityEngine;

public class VoiceExample : MonoBehaviour
{
    private UnityVoiceLiveClient client;

    void Start()
    {
        client = GetComponent<UnityVoiceLiveClient>();

        // Setup event listeners
        client.OnSessionStarted.AddListener(() =>
        {
            Debug.Log("Session started!");
        });

        client.OnTranscriptReceived.AddListener((transcript) =>
        {
            Debug.Log($"Transcript: {transcript}");
        });

        // Start connection
        _ = client.Connect();
    }
}
```

## Samples

The package includes the following samples:

### Avatar Sample

A sample demonstrating voice conversations with avatar video streaming.

**How to import:**
1. Select this package in Unity Package Manager
2. Import "Avatar Sample" from the "Samples" section

## Architecture

```
Unity MonoBehaviour Components
    ↓
UnityVoiceLiveClient (Main Thread Queue)
    ↓
VoiceLive API Core (.NET Standard 2.1)
    ↓
Microsoft Foundry WebSocket API
```

## Platform Support

| Platform | Support | Notes |
|:---|:---:|:---|
| Windows (Standalone) | ✅ | |
| macOS (Standalone) | ✅ | |
| Linux (Standalone) | ✅ | |
| Android | ✅ | Microphone permissions required |
| iOS | ✅ | Microphone permissions required |
| WebGL | ⚠️ | WebSocket support required (limited) |

## Documentation

For detailed documentation, see:

- [Package README](Unity/UnityVoiceLiveAPI/Assets/Reseul/UnityVoiceLiveAPI/README.md) - Detailed usage and API reference
- [CHANGELOG](CHANGELOG.md) - Release notes
- [THIRD-PARTY-NOTICES](THIRD-PARTY-NOTICES.md) - Third-party licenses

## License

[Boost Software License 1.0](LICENSE)

## Author

**Takahiro Miyaura**
- GitHub: [@TakahiroMiyaura](https://github.com/TakahiroMiyaura)

## Contributing

Issues and Pull Requests are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## Related Links

- [Microsoft Foundry](https://ai.azure.com/)
- [Azure AI VoiceLive API Documentation](https://learn.microsoft.com/azure/ai-services/)
- [Unity WebRTC Package](https://docs.unity3d.com/Packages/com.unity.webrtc@3.0/)
