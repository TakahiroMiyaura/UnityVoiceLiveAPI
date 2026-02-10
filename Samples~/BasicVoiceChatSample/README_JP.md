# Basic Voice Chat サンプル

[![English](https://img.shields.io/badge/lang-English-blue.svg)](README.md)

アバター機能を使用しない、UnityVoiceLiveClient の基本的な使用方法を示すシンプルな音声チャットサンプルです。

## 機能

- 接続/切断ボタン
- 録音開始/停止ボタン
- リアルタイムの文字起こし表示
- AI応答の表示
- 接続状態インジケーター

## セットアップ

### シーンのセットアップ

1. 新しいシーンを作成するか、既存のシーンを使用
2. "VoiceLiveManager" という名前の空の GameObject を作成
3. 以下のコンポーネントを追加:
   - `UnityVoiceLiveClient`
   - `BasicVoiceChatSample`
   - `AudioSource`（AI音声再生用）

### UIのセットアップ

以下のUI要素を作成し、Inspectorで割り当てます：

| フィールド | UI要素 | 説明 |
|:----------|:-------|:-----|
| Message Area | TextMeshProUGUI | 会話メッセージを表示 |
| Record Button | Button | 音声録音の開始/停止 |
| Record Button Text | TextMeshProUGUI | 録音ボタンのテキスト |
| Connect Button | Button | APIへの接続/切断 |
| Connect Button Text | TextMeshProUGUI | 接続ボタンのテキスト |
| Status Text | TextMeshProUGUI | 現在の接続状態を表示 |

### 設定

`UnityVoiceLiveClient` コンポーネントを設定:

1. **Endpoint** に Microsoft Foundry のエンドポイントURLを設定
2. **Access Token** に APIキーを設定
3. **Connection Mode** を選択（AIAgent または AIModel）
4. 再生用の **Audio Source** コンポーネントを割り当て

## 使い方

1. **Connect** を押して接続を確立
2. "Session active" ステータスを待つ
3. **Start Recording** を押して発話開始
4. メッセージを話す
5. 終わったら **Stop Recording** を押す
6. AI応答が自動的に再生される
7. 終了時は **Disconnect** を押す

## コード概要

```csharp
// APIに接続
await client.Connect();

// 音声録音を開始
client.StartRecording();

// 音声録音を停止
client.StopRecording();

// 切断
client.Disconnect();
```

## イベント

| イベント | 説明 |
|:--------|:-----|
| OnSessionStarted | セッションが会話可能な状態になった |
| OnTranscriptReceived | ユーザーの発話が文字起こしされた |
| OnResponseOutputItemDoneReceived | AI応答テキストを受信 |
| OnErrorOccurred | 操作中にエラーが発生 |
