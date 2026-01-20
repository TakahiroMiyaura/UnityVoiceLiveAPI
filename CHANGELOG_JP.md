# Changelog

[![English](https://img.shields.io/badge/lang-English-blue.svg)](CHANGELOG.md)

このプロジェクトのすべての注目すべき変更はこのファイルに記録されます。

フォーマットは [Keep a Changelog](https://keepachangelog.com/ja/1.1.0/) に基づいており、
このプロジェクトは [Semantic Versioning](https://semver.org/lang/ja/) に準拠しています。

## [Unreleased]

## [1.0.0] - 2026-01-20

### Added

- **UnityVoiceLiveClient**: Azure AI VoiceLive API との統合を行うメインコンポーネント
  - AI Agent モードと AI Model モードの両方に対応
  - WebSocket 通信による低遅延のリアルタイム音声処理
  - メインスレッドでのイベントハンドリング

- **オーディオ機能**
  - `UnityAudioCapture`: マイクからの音声キャプチャ（PCM16 形式、24kHz サンプルレート）
  - `UnityAudioPlayback`: AI レスポンスの音声再生（AudioClip への自動変換）

- **アバターサポート**
  - `UnityAvatarClient`: Unity WebRTC による映像ストリーミング
  - SDP/ICE による接続ネゴシエーション

- **設定システム** (ScriptableObject ベース)
  - `FoundryConnectionSettings`: 接続設定（エンドポイント、認証情報）
  - `VoiceLiveSessionSettings`: セッション全体の設定
  - `VoiceSettings`: 音声パラメータ（Azure TTS 音声選択等）
  - `AudioProcessingSettings`: オーディオ処理設定
  - `TurnDetectionSettings`: 音声ターン検出設定
  - `VoiceLiveAvatarSettings`: アバター固有設定

- **Editor 拡張**
  - カスタム Inspector による設定 UI
  - ドロップダウン属性によるプロパティ選択 UI

- **サンプル**
  - Avatar Sample: アバター映像ストリーミングを使用した音声会話サンプル

- **Unity イベントシステム**
  - `OnConnected`: 接続完了イベント
  - `OnDisconnected`: 切断イベント
  - `OnSessionStarted`: セッション開始イベント
  - `OnTranscriptReceived`: テキスト認識結果イベント
  - `OnErrorOccurred`: エラー発生イベント

- **プラットフォームサポート**
  - Windows, macOS, Linux (Standalone)
  - Android, iOS (マイク権限設定が必要)
  - WebGL (WebSocket サポート範囲内)

---

## バージョニングについて

- **MAJOR**: 互換性のない API 変更
- **MINOR**: 後方互換性のある機能追加
- **PATCH**: 後方互換性のあるバグ修正

[Unreleased]: https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI/releases/tag/v1.0.0
