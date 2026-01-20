# VoiceLive API for Unity

[![English](https://img.shields.io/badge/lang-English-blue.svg)](README.md)

Azure AI VoiceLive API の Unity 統合ライブラリです。AI モデル（GPT-4o等）やカスタム AI エージェントとのリアルタイム音声会話機能を Unity アプリケーションに組み込むことができます。

## 概要

このライブラリは [Microsoft Foundry](https://azure.microsoft.com/products/ai-foundry) の VoiceLive API を Unity から簡単に利用するためのラッパーを提供します。WebSocket 通信、音声のキャプチャ・再生、アバター映像ストリーミングなどの複雑な処理を抽象化し、Unity の Inspector から設定可能なコンポーネントとして提供します。

## 主な機能

- **リアルタイム音声会話**: Azure AI モデル（GPT-4o）やカスタムエージェントとの音声対話
- **Unity コンポーネント**: Inspector で設定可能な MonoBehaviour ベースのコンポーネント
- **WebSocket 統合**: メインスレッドでのイベントハンドリングに対応した効率的な WebSocket 通信
- **オーディオ処理**: マイク入力のキャプチャと AudioSource による音声再生（PCM16 形式対応）
- **アバターサポート**: Unity WebRTC によるアバター映像ストリーミング（オプション機能）

## 動作要件

- Unity 6000.0 以降
- .NET Standard 2.1
- Microsoft Foundry アカウント（VoiceLive API へのアクセス権限が必要）

### オプション依存関係

- `com.unity.webrtc` 3.0.0 以降（アバター機能を使用する場合）

## インストール方法

### Unity Package Manager 経由

1. Unity Package Manager を開く（Window → Package Manager）
2. 「+」ボタン → 「Add package from git URL」を選択
3. 以下のいずれかの URL を入力：

**最新版（upm ブランチ）:**
```
https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI.git#upm
```

**特定バージョン（例: 1.0.0）:**
```
https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI.git#upm@1.0.0
```

### 手動インストール

1. このリポジトリをクローンまたはダウンロード
2. `Unity/UnityVoiceLiveAPI/Assets/Reseul/UnityVoiceLiveAPI` フォルダを Unity プロジェクトの `Packages` ディレクトリにコピー
3. 必要な依存関係がインストールされていることを確認：
   - `com.unity.nuget.newtonsoft-json` (3.2.1 以降)

## クイックスタート

### 基本的な使い方

1. シーンに空の GameObject を作成
2. `UnityVoiceLiveClient` コンポーネントをアタッチ
3. Inspector で接続設定を構成：
   - Endpoint: Azure AI のエンドポイント URL
   - Access Token: API キーまたは Bearer トークン
   - Connection Mode: AIAgent または AIModel を選択

### コード例

```csharp
using Com.Reseul.Azure.AI.VoiceLiveAPI.Unity.Components;
using UnityEngine;

public class VoiceExample : MonoBehaviour
{
    private UnityVoiceLiveClient client;

    void Start()
    {
        client = GetComponent<UnityVoiceLiveClient>();

        // イベントリスナーの設定
        client.OnSessionStarted.AddListener(() =>
        {
            Debug.Log("セッション開始！");
        });

        client.OnTranscriptReceived.AddListener((transcript) =>
        {
            Debug.Log($"認識結果: {transcript}");
        });

        // 接続開始
        _ = client.Connect();
    }
}
```

## サンプル

パッケージには以下のサンプルが含まれています：

### Avatar Sample

アバター映像ストリーミングを使用した音声会話のサンプルです。

**インポート方法:**
1. Unity Package Manager でこのパッケージを選択
2. 「Samples」セクションから「Avatar Sample」をインポート

## アーキテクチャ

```
Unity MonoBehaviour Components
    ↓
UnityVoiceLiveClient (メインスレッドキュー)
    ↓
VoiceLive API Core (.NET Standard 2.1)
    ↓
Microsoft Foundry WebSocket API
```

## プラットフォームサポート

| プラットフォーム | サポート状況 | 備考 |
|:---|:---:|:---|
| Windows (Standalone) | ✅ | |
| macOS (Standalone) | ✅ | |
| Linux (Standalone) | ✅ | |
| Android | ✅ | マイク権限の設定が必要 |
| iOS | ✅ | マイク権限の設定が必要 |
| WebGL | ⚠️ | WebSocket サポートが必要（制限あり） |

## ドキュメント

詳細なドキュメントは以下を参照してください：

- [パッケージ README](Unity/UnityVoiceLiveAPI/Assets/Reseul/UnityVoiceLiveAPI/README.md) - 詳細な使用方法とAPI リファレンス
- [CHANGELOG](CHANGELOG_JP.md) - 更新履歴
- [THIRD-PARTY-NOTICES](THIRD-PARTY-NOTICES.md) - サードパーティライセンス

## ライセンス

[Boost Software License 1.0](LICENSE)

## 作者

**Takahiro Miyaura**
- GitHub: [@TakahiroMiyaura](https://github.com/TakahiroMiyaura)

## 貢献

Issue や Pull Request は歓迎します。詳細は [CONTRIBUTING_JP.md](CONTRIBUTING_JP.md) を参照してください。

## 関連リンク

- [Microsoft Foundry](https://azure.microsoft.com/products/ai-foundry)
- [Azure AI VoiceLive API ドキュメント](https://learn.microsoft.com/azure/ai-services/)
- [Unity WebRTC Package](https://docs.unity3d.com/Packages/com.unity.webrtc@3.0/)
