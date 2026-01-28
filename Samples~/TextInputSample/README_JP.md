# Text Input サンプル

[![English](https://img.shields.io/badge/lang-English-blue.svg)](README.md)

テキストベースのAI会話を示すサンプルです。オプションで音声入力もサポートしています。

## 機能

- メッセージ入力用のテキストフィールド
- 送信ボタンとEnterキーのサポート
- オプションの音声入力トグル
- リアルタイムの文字起こし表示（音声入力時）
- AI応答の表示と音声再生
- 接続状態インジケーター

## セットアップ

### シーンのセットアップ

1. 新しいシーンを作成するか、既存のシーンを使用
2. "VoiceLiveManager" という名前の空の GameObject を作成
3. 以下のコンポーネントを追加:
   - `UnityVoiceLiveClient`
   - `TextInputSample`
   - `AudioSource`（AI音声再生用）

### UIのセットアップ

以下のUI要素を作成し、Inspectorで割り当てます：

| フィールド | UI要素 | 説明 |
|:----------|:-------|:-----|
| Message Area | TextMeshProUGUI | 会話メッセージを表示 |
| Input Field | TMP_InputField | メッセージ入力用テキストフィールド |
| Send Button | Button | 入力したメッセージを送信 |
| Send Button Text | TextMeshProUGUI | 送信ボタンのテキスト |
| Connect Button | Button | APIへの接続/切断 |
| Connect Button Text | TextMeshProUGUI | 接続ボタンのテキスト |
| Status Text | TextMeshProUGUI | 現在の接続状態を表示 |
| Voice Input Toggle | Toggle | オプション: 音声入力の有効化 |

### 設定

`UnityVoiceLiveClient` コンポーネントを設定:

1. **Endpoint** に Microsoft Foundry のエンドポイントURLを設定
2. **Access Token** に APIキーを設定
3. **Connection Mode** を選択（AIAgent または AIModel）
4. 再生用の **Audio Source** コンポーネントを割り当て

## 使い方

### テキスト入力

1. **Connect** を押して接続を確立
2. "Session active" ステータスを待つ
3. 入力フィールドにメッセージを入力
4. **Send** を押すか Enter キーを押す
5. AI応答が自動的に再生される

### 音声入力（オプション）

1. **Voice Input Toggle** を有効にする
2. メッセージを話す
3. トグルをオフにして音声入力を停止
4. テキストと音声メッセージは混在可能

## コード概要

```csharp
// APIに接続
await client.Connect();

// テキストメッセージを送信
client.SendTextMessage("こんにちは、調子はどうですか？");

// 音声入力を有効化
client.StartRecording();

// 音声入力を無効化
client.StopRecording();
```

## イベント

| イベント | 説明 |
|:--------|:-----|
| OnSessionStarted | セッションが会話可能な状態になった |
| OnTranscriptReceived | ユーザーの発話が文字起こしされた（音声入力） |
| OnResponseOutputItemDoneReceived | AI応答テキストを受信 |
| OnErrorOccurred | 操作中にエラーが発生 |

## 備考

- テキストメッセージは `[You (text)]` プレフィックスで表示
- 音声の文字起こしは `[You (voice)]` プレフィックスで表示
- AI応答は `[AI]` プレフィックスで表示
- 同じセッション内で両方の入力方法を使用可能
