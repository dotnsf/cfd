using CfdApp.Models;
using CfdApp.UI;
using CfdApp.Operations;

namespace CfdApp.Input;

/// <summary>
/// キーボード入力を処理
/// </summary>
public class KeyHandler
{
    /// <summary>
    /// キー入力を処理
    /// </summary>
    public static void HandleKey(AppState state, ConsoleKeyInfo keyInfo)
    {
        // 現在の画面モードに応じて処理を分岐
        switch (state.CurrentScreen)
        {
            case ScreenMode.FileBrowser:
                HandleFileBrowserKey(state, keyInfo);
                break;
            case ScreenMode.TextViewer:
                HandleTextViewerKey(state, keyInfo);
                break;
            case ScreenMode.Dialog:
                HandleDialogKey(state, keyInfo);
                break;
        }
    }

    private static void HandleFileBrowserKey(AppState state, ConsoleKeyInfo keyInfo)
    {
        var dirState = state.DirectoryState;
        int entriesPerPage = state.GetTotalEntriesPerPage();
        int rowsPerColumn = state.GetEntriesPerPage();

        switch (keyInfo.Key)
        {
            // 上移動（矢印キーまたは k）
            case ConsoleKey.UpArrow when keyInfo.Modifiers == 0:
            case ConsoleKey.K when keyInfo.Modifiers == 0:
                if (dirState.SelectedIndex > 0)
                {
                    dirState.SelectedIndex--;
                    // ページの先頭を超えた場合、前のページに移動
                    if (dirState.SelectedIndex < dirState.PageStartIndex)
                    {
                        dirState.PageStartIndex = Math.Max(0, dirState.PageStartIndex - entriesPerPage);
                    }
                }
                break;

            // 下移動（矢印キーまたは j）
            case ConsoleKey.DownArrow when keyInfo.Modifiers == 0:
            case ConsoleKey.J when keyInfo.Modifiers == 0:
                if (dirState.SelectedIndex < dirState.Entries.Count - 1)
                {
                    dirState.SelectedIndex++;
                    // ページの最後を超えた場合、次のページに移動
                    if (dirState.SelectedIndex >= dirState.PageStartIndex + entriesPerPage)
                    {
                        dirState.PageStartIndex += entriesPerPage;
                    }
                }
                break;

            // 左移動（矢印キーまたは h）- 前のカラムに移動
            case ConsoleKey.LeftArrow when keyInfo.Modifiers == 0:
            case ConsoleKey.H when keyInfo.Modifiers == 0:
                if (dirState.SelectedIndex >= rowsPerColumn)
                {
                    dirState.SelectedIndex -= rowsPerColumn;
                }
                break;

            // 右移動（矢印キーまたは l）- 次のカラムに移動
            case ConsoleKey.RightArrow when keyInfo.Modifiers == 0:
            case ConsoleKey.L when keyInfo.Modifiers == 0:
                if (dirState.SelectedIndex + rowsPerColumn < dirState.Entries.Count)
                {
                    dirState.SelectedIndex += rowsPerColumn;
                }
                break;

            // ページアップ
            case ConsoleKey.PageUp:
                dirState.PageStartIndex = Math.Max(0, dirState.PageStartIndex - entriesPerPage);
                dirState.SelectedIndex = dirState.PageStartIndex;
                break;

            // ページダウン
            case ConsoleKey.PageDown:
                int maxPage = Math.Max(0, dirState.Entries.Count - entriesPerPage);
                dirState.PageStartIndex = Math.Min(maxPage, dirState.PageStartIndex + entriesPerPage);
                dirState.SelectedIndex = dirState.PageStartIndex;
                break;

            // Enter - ディレクトリに入る、またはファイルを開く
            case ConsoleKey.Enter:
                HandleEnter(state);
                break;

            // カラム数変更
            case ConsoleKey.D2:
                state.ColumnCount = 2;
                break;
            case ConsoleKey.D3:
                state.ColumnCount = 3;
                break;
            case ConsoleKey.D5:
                state.ColumnCount = 5;
                break;

            // ファイル操作
            case ConsoleKey.C when keyInfo.Modifiers == 0:
                state.CurrentDialog = DialogMode.CopyDestination;
                state.CurrentScreen = ScreenMode.Dialog;
                state.DialogInput = string.Empty;
                break;

            case ConsoleKey.M when keyInfo.Modifiers == 0:
                state.CurrentDialog = DialogMode.MoveDestination;
                state.CurrentScreen = ScreenMode.Dialog;
                state.DialogInput = string.Empty;
                break;

            case ConsoleKey.D when keyInfo.Modifiers == 0:
                state.CurrentDialog = DialogMode.DeleteConfirm;
                state.CurrentScreen = ScreenMode.Dialog;
                break;

            case ConsoleKey.X when keyInfo.Modifiers == 0:
                HandleExecute(state);
                break;

            // 終了
            case ConsoleKey.Q when keyInfo.Modifiers == 0:
            case ConsoleKey.Escape when state.CurrentScreen == ScreenMode.FileBrowser:
                state.ShouldExit = true;
                break;
        }
    }

    private static void HandleTextViewerKey(AppState state, ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
            case ConsoleKey.K:
                TextViewer.ScrollUp(state);
                break;

            case ConsoleKey.DownArrow:
            case ConsoleKey.J:
                TextViewer.ScrollDown(state);
                break;

            case ConsoleKey.PageUp:
                TextViewer.PageUp(state);
                break;

            case ConsoleKey.PageDown:
                TextViewer.PageDown(state);
                break;

            case ConsoleKey.Escape:
                state.CurrentScreen = ScreenMode.FileBrowser;
                state.TextViewerScrollOffset = 0;
                break;
        }
    }

    private static void HandleDialogKey(AppState state, ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                HandleDialogConfirm(state);
                break;

            case ConsoleKey.Escape:
                DialogRenderer.ClearDialog(state);
                break;

            case ConsoleKey.Backspace:
                DialogRenderer.RemoveChar(state);
                break;

            default:
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    DialogRenderer.AddChar(state, keyInfo.KeyChar);
                }
                break;
        }
    }

    private static void HandleEnter(AppState state)
    {
        var entry = state.DirectoryState.GetSelectedEntry();
        if (entry == null) return;

        if (entry.IsDirectory)
        {
            // ディレクトリに入る
            state.DirectoryState.LoadDirectory(entry.FullPath);
        }
        else if (entry.IsTextFile())
        {
            // テキストファイルを開く
            if (TextViewer.LoadFile(entry.FullPath))
            {
                state.TextViewerFilePath = entry.FullPath;
                state.TextViewerScrollOffset = 0;
                state.CurrentScreen = ScreenMode.TextViewer;
            }
        }
    }

    private static void HandleExecute(AppState state)
    {
        var entry = state.DirectoryState.GetSelectedEntry();
        if (entry == null || !entry.IsExecutable()) return;

        FileExecutor.Execute(entry.FullPath);
        
        // 実行後、画面を再描画
        state.DirectoryState.LoadDirectory(state.DirectoryState.CurrentPath);
    }

    private static void HandleDialogConfirm(AppState state)
    {
        var entry = state.DirectoryState.GetSelectedEntry();
        if (entry == null)
        {
            DialogRenderer.ClearDialog(state);
            return;
        }

        switch (state.CurrentDialog)
        {
            case DialogMode.CopyDestination:
                if (!string.IsNullOrWhiteSpace(state.DialogInput))
                {
                    FileOperations.Copy(entry.FullPath, state.DialogInput, entry.IsDirectory);
                    state.DirectoryState.LoadDirectory(state.DirectoryState.CurrentPath);
                }
                break;

            case DialogMode.MoveDestination:
                if (!string.IsNullOrWhiteSpace(state.DialogInput))
                {
                    FileOperations.Move(entry.FullPath, state.DialogInput, entry.IsDirectory);
                    state.DirectoryState.LoadDirectory(state.DirectoryState.CurrentPath);
                }
                break;

            case DialogMode.DeleteConfirm:
                FileOperations.Delete(entry.FullPath, entry.IsDirectory);
                state.DirectoryState.LoadDirectory(state.DirectoryState.CurrentPath);
                break;
        }

        DialogRenderer.ClearDialog(state);
    }
}

// Made with Bob
