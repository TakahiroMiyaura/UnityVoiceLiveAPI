# Changelog


[![日本語](https://img.shields.io/badge/lang-日本語-red.svg)](CHANGELOG_JP.md)

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0-pre.2] - 2026-01-20

### Added

- **UnityVoiceLiveClient**: Main component for Azure AI VoiceLive API integration
  - Support for both AI Agent mode and AI Model mode
  - Low-latency real-time audio processing via WebSocket
  - Main thread event handling

- **Audio Features**
  - `UnityAudioCapture`: Microphone audio capture (PCM16 format, 24kHz sample rate)
  - `UnityAudioPlayback`: AI response audio playback (automatic AudioClip conversion)

- **Avatar Support**
  - `UnityAvatarClient`: Video streaming via Unity WebRTC
  - SDP/ICE connection negotiation

- **Settings System** (ScriptableObject-based)
  - `FoundryConnectionSettings`: Connection settings (endpoint, authentication)
  - `VoiceLiveSessionSettings`: Overall session configuration
  - `VoiceSettings`: Voice parameters (Azure TTS voice selection, etc.)
  - `AudioProcessingSettings`: Audio processing configuration
  - `TurnDetectionSettings`: Voice turn detection settings
  - `VoiceLiveAvatarSettings`: Avatar-specific settings

- **Editor Extensions**
  - Custom Inspector UI for settings
  - Dropdown attribute for property selection UI

- **Samples**
  - Avatar Sample: Voice conversation sample with avatar video streaming

- **Unity Event System**
  - `OnConnected`: Connection established event
  - `OnDisconnected`: Disconnection event
  - `OnSessionStarted`: Session start event
  - `OnTranscriptReceived`: Text transcript received event
  - `OnErrorOccurred`: Error occurred event

- **Platform Support**
  - Windows, macOS, Linux (Standalone)
  - Android, iOS (microphone permissions required)
  - WebGL (within WebSocket support scope)

---

## About Versioning

- **MAJOR**: Incompatible API changes
- **MINOR**: Backward-compatible feature additions
- **PATCH**: Backward-compatible bug fixes

[Unreleased]: https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI/compare/v1.0.0-pre.2...HEAD
[1.0.0-pre.2]: https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI/releases/tag/v1.0.0-pre.2
