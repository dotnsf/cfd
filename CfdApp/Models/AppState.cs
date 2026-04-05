namespace CfdApp.Models;

/// <summary>
/// アプリケーション全体の状態を管理
/// </summary>
public class AppState
{
    public DirectoryState DirectoryState { get; set; } = new();
    public ScreenMode CurrentScreen { get; set; } = ScreenMode.FileBrowser;
    public int ColumnCount { get; set; } = 2; // デフォルトは2カラム
    public string? TextViewerFilePath { get; set; }
    public int TextViewerScrollOffset { get; set; } = 0;
    public DialogMode CurrentDialog { get; set; } = DialogMode.None;
    public string DialogInput { get; set; } = string.Empty;
    public bool ShouldExit { get; set; } = false;

    /// <summary>
    /// 画面の高さから1ページあたりの表示可能なエントリ数を計算
    /// </summary>
    public int GetEntriesPerPage()
    {
        // ヘッダー(2行) + フッター(1行) を除いた行数
        int availableLines = Console.WindowHeight - 3;
        // カラム数で割って、各カラムの行数を取得
        return availableLines;
    }

    /// <summary>
    /// 1ページあたりの総エントリ数（全カラム合計）
    /// </summary>
    public int GetTotalEntriesPerPage()
    {
        return GetEntriesPerPage() * ColumnCount;
    }
}

/// <summary>
/// 画面モード
/// </summary>
public enum ScreenMode
{
    FileBrowser,    // ファイルブラウザ
    TextViewer,     // テキストビューア
    Dialog          // ダイアログ表示中
}

/// <summary>
/// ダイアログモード
/// </summary>
public enum DialogMode
{
    None,           // ダイアログなし
    CopyDestination,// コピー先入力
    MoveDestination,// 移動先入力
    DeleteConfirm   // 削除確認
}

// Made with Bob
