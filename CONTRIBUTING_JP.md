# Contributing / 貢献ガイド

[![English](https://img.shields.io/badge/lang-English-blue.svg)](CONTRIBUTING.md)

VoiceLive API for Unity への貢献に興味を持っていただきありがとうございます。

## 貢献の方法

### Issue の報告

バグ報告や機能リクエストは [GitHub Issues](https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI/issues) で受け付けています。

Issue を作成する際は、以下の情報を含めてください：

- **バグ報告の場合**
  - 問題の詳細な説明
  - 再現手順
  - 期待される動作と実際の動作
  - Unity バージョン
  - 対象プラットフォーム
  - 関連するエラーメッセージやログ

- **機能リクエストの場合**
  - 提案する機能の説明
  - ユースケースや背景
  - 可能であれば実装案

### Pull Request

1. このリポジトリをフォークしてください
2. 機能ブランチを作成してください (`git checkout -b feature/amazing-feature`)
3. 変更をコミットしてください (`git commit -m 'Add amazing feature'`)
4. ブランチにプッシュしてください (`git push origin feature/amazing-feature`)
5. Pull Request を作成してください

#### Pull Request のガイドライン

- 1つの PR には1つの機能・修正のみを含めてください
- 既存のコードスタイルに従ってください
- 必要に応じてドキュメントを更新してください
- テストがある場合は、テストが通ることを確認してください

## 開発環境のセットアップ

### 必要なもの

- Unity 6000.0 以降
- .NET SDK（.NET Standard 2.1 対応）
- Git

### ビルド手順

1. リポジトリをクローン
   ```bash
   git clone https://github.com/TakahiroMiyaura/UnityVoiceLiveAPI.git
   ```

2. Unity で `Unity/UnityVoiceLiveAPI` プロジェクトを開く

3. 必要なパッケージが自動的にインストールされます

## コーディング規約

- C# コーディング規約に従ってください
- public メンバーには XML ドキュメントコメントを追加してください
- 意味のある変数名・メソッド名を使用してください

## ライセンス

このプロジェクトに貢献することで、あなたの貢献は [Boost Software License 1.0](LICENSE) の下でライセンスされることに同意したものとみなされます。

## 行動規範

コミュニティの一員として、お互いを尊重し、建設的なコミュニケーションを心がけてください。

## 質問

質問がある場合は、Issue を作成するか、作者に直接お問い合わせください。
