# Avatar サンプル

[![English](https://img.shields.io/badge/lang-English-blue.svg)](README.md)

WebRTCを使用したリアルタイムビデオ・オーディオストリーミングにより、UnityVoiceLiveClientとAzure AI Avatarを統合するサンプルです。

## 機能

- シーン開始時の自動接続
- WebRTC経由のAI Avatarビデオストリーミング
- リアルタイムのリップシンクとアバターアニメーション
- ライブ文字起こし付き音声入力
- アバター音声応答の再生
- メッセージ表示エリア

## 前提条件

このサンプルには **Unity WebRTC** パッケージのインストールが必要です：

1. **Window > Package Manager** を開く
2. **+** をクリックし、**Add package from git URL...** を選択
3. 入力: `com.unity.webrtc`
4. **Add** をクリック

## セットアップ

### シーンのセットアップ

1. 新しいシーンを作成するか、提供された `Avatar.unity` を使用
2. "VoiceLiveManager" という名前の空の GameObject を作成
3. 以下のコンポーネントを追加:
   - `UnityVoiceLiveClient`
   - `UnityAvatarClient`
   - `AgentSample`
   - `AudioSource`（アバター音声再生用）

### UIのセットアップ

以下のUI要素を作成し、Inspectorで割り当てます：

#### AgentSample コンポーネント

| フィールド | UI要素 | 説明 |
|:----------|:-------|:-----|
| Message Area | TextMeshProUGUI | 会話メッセージを表示 |

#### UnityAvatarClient コンポーネント

| フィールド | UI要素 | 説明 |
|:----------|:-------|:-----|
| Video Display | RawImage | アバタービデオストリームを表示 |
| Audio Source | AudioSource | アバター音声ストリームを再生 |

### 設定

#### UnityVoiceLiveClient コンポーネント

1. **Endpoint** に Microsoft Foundry のエンドポイントURLを設定
2. **Access Token** に APIキーを設定
3. **Connection Mode** を選択（AIAgent または AIModel）
4. 再生用の **Audio Source** コンポーネントを割り当て
5. **重要**: アバターモードを有効にするため **Avatar Settings** アセットを割り当て

#### Avatar Settings

`VoiceLiveAvatarSettings` アセットを作成または既存のものを使用：

1. Project ウィンドウで右クリック
2. **Create > Microsoft Foundry > VoiceLive API > Avatar Settings** を選択
3. 以下を設定:

| 設定 | 説明 |
|:----|:-----|
| Avatar Preset | キャラクターとスタイル（例: "lisa:casual-sitting"） |
| Video Settings | 解像度とフォーマットオプション |
| Background Settings | アバター背景の設定 |

## 使い方

1. Microsoft Foundry 接続設定を構成
2. Play モードに入る
3. サンプルが自動的にAPIに接続
4. "Session started" メッセージを待つ
5. 話し始める - 音声が文字起こしされます
6. AI Avatar がリップシンク付きで応答
7. 会話履歴がメッセージエリアに表示されます

## コード概要

```csharp
// AgentSample は Start で自動接続
private async void Start()
{
    client = GetComponent<UnityVoiceLiveClient>();

    // イベントリスナーをセットアップ
    client.OnSessionStarted.AddListener(HandleSessionStarted);
    client.OnTranscriptReceived.AddListener(HandleTranscript);
    client.OnResponseOutputItemDoneReceived.AddListener(AddMessage);
    client.OnErrorOccurred.AddListener(HandleError);

    // Azure AI VoiceLive API に接続
    await client.Connect();
}
```

## イベント

### AgentSample イベント

| イベント | 説明 |
|:--------|:-----|
| OnSessionStarted | セッションが会話可能な状態になった |
| OnTranscriptReceived | ユーザーの発話が文字起こしされた |
| OnResponseOutputItemDoneReceived | AI応答テキストを受信 |
| OnErrorOccurred | 操作中にエラーが発生 |

### UnityAvatarClient イベント

| イベント | 説明 |
|:--------|:-----|
| OnConnectionEstablished | WebRTC接続が確立された |
| OnConnectionClosed | WebRTC接続が閉じられた |
| OnVideoFrameReceived | アバターからビデオフレームを受信 |
| OnAudioFrameReceived | アバターからオーディオフレームを受信 |
| OnError | アバタークライアントでエラーが発生 |

## 利用可能なアバタープリセット

一般的なアバターキャラクター:スタイルの組み合わせ：

- `lisa:casual-sitting`
- `lisa:graceful-sitting`
- `lisa:graceful-standing`
- `harry:casual-sitting`
- `harry:graceful-sitting`
- `harry:graceful-standing`

利用可能なプリセットの完全なリストについては、Azure AI Avatar のドキュメントを参照してください。

## トラブルシューティング

### アバタービデオが表示されない
- Unity WebRTC パッケージがインストールされていることを確認
- RawImage が Video Display に割り当てられていることを確認
- Avatar Settings アセットが UnityVoiceLiveClient に割り当てられていることを確認

### 音声が再生されない
- AudioSource が UnityAvatarClient に割り当てられていることを確認
- AudioSource の音量とミュート設定を確認

### 接続に失敗する
- Microsoft Foundry のエンドポイントとアクセストークンを確認
- Foundry の設定でアバター機能が有効になっていることを確認
