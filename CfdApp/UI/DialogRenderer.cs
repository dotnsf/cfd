using CfdApp.Models;

namespace CfdApp.UI;

/// <summary>
/// ダイアログを描画
/// </summary>
public class DialogRenderer
{
    /// <summary>
    /// ダイアログを描画
    /// </summary>
    public static void Render(AppState state)
    {
        // ファイルリストを背景として描画
        FileListRenderer.Render(state);
        
        // ダイアログボックスを上に重ねて描画
        RenderDialogBox(state);
    }

    private static void RenderDialogBox(AppState state)
    {
        int boxWidth = 60;
        int boxHeight = 5;
        int startX = (Console.WindowWidth - boxWidth) / 2;
        int startY = (Console.WindowHeight - boxHeight) / 2;
        
        // ダイアログの種類に応じてメッセージを設定
        string title = state.CurrentDialog switch
        {
            DialogMode.CopyDestination => "コピー先ディレクトリを入力",
            DialogMode.MoveDestination => "移動先ディレクトリを入力",
            DialogMode.DeleteConfirm => "削除の確認",
            _ => ""
        };
        
        string message = state.CurrentDialog switch
        {
            DialogMode.CopyDestination => "コピー先:",
            DialogMode.MoveDestination => "移動先:",
            DialogMode.DeleteConfirm => GetDeleteConfirmMessage(state),
            _ => ""
        };
        
        // ダイアログボックスを描画
        DrawBox(startX, startY, boxWidth, boxHeight, title);
        
        // メッセージを描画
        Console.SetCursorPosition(startX + 2, startY + 2);
        Console.Write(message);
        
        // 入力フィールドまたは確認メッセージを描画
        if (state.CurrentDialog == DialogMode.DeleteConfirm)
        {
            Console.SetCursorPosition(startX + 2, startY + 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter:削除 ESC:キャンセル");
            Console.ResetColor();
        }
        else
        {
            Console.SetCursorPosition(startX + 2, startY + 3);
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            string input = state.DialogInput.PadRight(boxWidth - 4);
            if (input.Length > boxWidth - 4)
            {
                input = input.Substring(0, boxWidth - 4);
            }
            Console.Write(input);
            Console.ResetColor();
            
            // カーソルを入力位置に設定
            Console.SetCursorPosition(startX + 2 + state.DialogInput.Length, startY + 3);
        }
    }

    private static void DrawBox(int x, int y, int width, int height, string title)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        
        // 上辺
        Console.SetCursorPosition(x, y);
        Console.Write("┌" + new string('─', width - 2) + "┓");
        
        // タイトル
        if (!string.IsNullOrEmpty(title))
        {
            int titleX = x + (width - title.Length) / 2;
            Console.SetCursorPosition(titleX, y);
            Console.Write(title);
        }
        
        // 中央部
        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write("│" + new string(' ', width - 2) + "│");
        }
        
        // 下辺
        Console.SetCursorPosition(x, y + height - 1);
        Console.Write("└" + new string('─', width - 2) + "┛");
        
        Console.ResetColor();
    }

    private static string GetDeleteConfirmMessage(AppState state)
    {
        var entry = state.DirectoryState.GetSelectedEntry();
        if (entry == null) return "";
        
        string type = entry.IsDirectory ? "ディレクトリ" : "ファイル";
        return $"{type} '{entry.Name}' を削除しますか？";
    }

    /// <summary>
    /// ダイアログ入力に文字を追加
    /// </summary>
    public static void AddChar(AppState state, char c)
    {
        if (state.CurrentDialog != DialogMode.DeleteConfirm)
        {
            state.DialogInput += c;
        }
    }

    /// <summary>
    /// ダイアログ入力から文字を削除
    /// </summary>
    public static void RemoveChar(AppState state)
    {
        if (state.CurrentDialog != DialogMode.DeleteConfirm && state.DialogInput.Length > 0)
        {
            state.DialogInput = state.DialogInput.Substring(0, state.DialogInput.Length - 1);
        }
    }

    /// <summary>
    /// ダイアログをクリア
    /// </summary>
    public static void ClearDialog(AppState state)
    {
        state.CurrentDialog = DialogMode.None;
        state.DialogInput = string.Empty;
        state.CurrentScreen = ScreenMode.FileBrowser;
    }
}

// Made with Bob
