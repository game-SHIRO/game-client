# SHIRO - Game Client
<div align="right">
  最終更新日 08/04/26
</div>

---

## プロジェクト概要

SHIROはUnityで開発するTPSゲームのクライアントプロジェクトです。

---

## 開発環境

| 項目 | バージョン |
|------|-----------|
| Unity | Unity 6 LTS |
| C# | Unity標準 |
| Git | 最新版推奨 |
| Visual Studio Code | 最新版推奨 |

---

## 必要なツール

- Unity Hub
- Unity 6 LTS
- Git
- Visual Studio Code

---

## 初回セットアップ

### 1. リポジトリをクローン

```bash
git clone git@github.com:game-SHIRO/game-client.git
```

### 2. Unity Hubでプロジェクトを追加

「Add Project」からクローンしたフォルダを選択します。

### 3. Unityを起動

初回起動時はLibraryフォルダの生成に時間がかかります。

---

## ブランチ運用

### main

リリース用

### develop

開発統合用

### feature/*

機能開発用

例

```
feature/player
feature/ui
feature/weapon
feature/network
```

---

## 開発手順

最新の状態を取得

```bash
git checkout develop
git pull
```

機能ブランチを作成

```bash
git checkout -b feature/〇〇
```

作業終了後

```bash
git add .
git commit -m "プレイヤー移動を実装"
git push origin feature/〇〇
```

GitHubでPull Requestを作成してください。

---

## フォルダ構成

```
Assets/
├── Animations
├── Audio
├── Materials
├── Models
├── Prefabs
├── Scenes
├── Scripts
│   ├── Common
│   ├── Player
│   ├── UI
│   ├── Weapon
│   └── Network
├── Textures
└── UI
```

---

## コーディングルール

- クラス名はPascalCase
- メソッド名はPascalCase
- 変数名はcamelCase
- コメントは必要最低限
- publicよりSerializeFieldを優先する

---

## コミットメッセージ

例

```
feat: プレイヤー移動を実装
fix: ジャンプできない不具合を修正
refactor: Playerクラスを整理
docs: READMEを更新
```

---

## 注意事項

以下はGit管理対象外です。

- Library
- Temp
- Logs
- UserSettings

.gitignoreを変更する場合は事前に相談してください。

---

## ライセンス

本プロジェクトは関係者のみ利用可能です。
