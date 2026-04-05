# CFD - Console File Directory Manager

FDclone スタイルのコマンドラインファイラー for Windows

## 概要

CFD (Console File Directory Manager) は、Windows のコマンドプロンプト内で動作する高機能なファイルマネージャーです。UNIX の FDclone を参考に、直感的な操作性と豊富な機能を提供します。

## 特徴

- **マルチカラム表示**: 2/3/5 カラムの切り替え可能
- **詳細情報表示**: ファイル名、サイズ、更新日時を表示
- **vi キーバインド**: 矢印キーに加えて hjkl での移動をサポート
- **テキストビューア**: 各種テキストファイルの閲覧機能
- **ファイル操作**: コピー、移動、削除機能
- **プログラム実行**: .exe, .bat, .cmd ファイルの実行
- **単一実行ファイル**: .NET ランタイムを内蔵し、cfd.exe だけで動作

## インストール

1. `cfd/CfdApp/bin/Release/net8.0/win-x64/publish/cfd.exe` を任意のディレクトリにコピー
2. PATH に追加（オプション）

## 使い方

### 起動

```cmd
cfd.exe [ディレクトリパス]
```

ディレクトリパスを省略すると、カレントディレクトリで起動します。

### キーバインド

#### ナビゲーション
- **↑/↓ または k/j**: 上下移動
- **←/→ または h/l**: 左右移動（カラム間移動）
- **PgUp/PgDn**: ページ移動
- **Enter**: ディレクトリに入る / ファイルを開く

#### 表示切り替え
- **2**: 2 カラム表示
- **3**: 3 カラム表示
- **5**: 5 カラム表示

#### ファイル操作
- **c**: コピー（コピー先ディレクトリを入力）
- **m**: 移動（移動先ディレクトリを入力）
- **d**: 削除（確認ダイアログ表示）
- **x**: 実行可能ファイルを実行

#### その他
- **ESC**: 戻る / キャンセル / 終了
- **q**: 終了

### テキストビューア

テキストファイル（.txt, .md, .log, .cs など）を選択して Enter キーを押すと、テキストビューアが開きます。

- **↑/↓ または k/j**: スクロール
- **PgUp/PgDn**: ページスクロール
- **ESC**: ファイルブラウザに戻る

### ダイアログ操作

コピー・移動先の入力ダイアログでは：
- 文字入力でパスを指定
- **Backspace**: 文字削除
- **Enter**: 確定
- **ESC**: キャンセル

削除確認ダイアログでは：
- **Enter**: 削除実行
- **ESC**: キャンセル

## ビルド方法

```cmd
cd cfd/CfdApp
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

実行ファイルは `bin/Release/net8.0/win-x64/publish/cfd.exe` に生成されます。

## 技術仕様

- **言語**: C# (.NET 8.0)
- **ターゲット**: Windows 64bit
- **配布形式**: 単一実行ファイル（.NET ランタイム内蔵）
- **ファイルサイズ**: 約 70MB（圧縮済み）

## プロジェクト構造

```
cfd/
├── CfdApp/
│   ├── Models/           # データモデル
│   │   ├── FileEntry.cs
│   │   ├── DirectoryState.cs
│   │   └── AppState.cs
│   ├── UI/               # UI レンダリング
│   │   ├── FileListRenderer.cs
│   │   ├── TextViewer.cs
│   │   └── DialogRenderer.cs
│   ├── Input/            # 入力処理
│   │   └── KeyHandler.cs
│   ├── Operations/       # ファイル操作
│   │   ├── FileOperations.cs
│   │   └── FileExecutor.cs
│   └── Program.cs        # エントリーポイント
└── README.md
```

## ライセンス

このプロジェクトは個人利用・商用利用ともに自由に使用できます。

## 参考

- [FDclone](https://github.com/knu/FDclone) - オリジナルの UNIX ファイラー