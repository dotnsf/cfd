using CfdApp.Models;

namespace CfdApp.UI;

/// <summary>
/// ファイルリストを2/3/5カラムで表示するレンダラー
/// </summary>
public class FileListRenderer
{
    /// <summary>
    /// ファイルリストを描画
    /// </summary>
    public static void Render(AppState state)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        
        // ヘッダーを描画
        RenderHeader(state);
        
        // ファイルリストを描画
        RenderFileList(state);
        
        // フッターを描画
        RenderFooter(state);
    }

    private static void RenderHeader(AppState state)
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        
        var header = $" CFD - {state.DirectoryState.CurrentPath}";
        Console.Write(header.PadRight(Console.WindowWidth));
        
        Console.SetCursorPosition(0, 1);
        Console.ResetColor();
        Console.Write(new string('─', Console.WindowWidth));
    }

    private static void RenderFileList(AppState state)
    {
        var entries = state.DirectoryState.Entries;
        var selectedIndex = state.DirectoryState.SelectedIndex;
        var pageStart = state.DirectoryState.PageStartIndex;
        var entriesPerPage = state.GetTotalEntriesPerPage();
        
        // カラム幅を計算
        int columnWidth = Console.WindowWidth / state.ColumnCount;
        int rowsPerColumn = state.GetEntriesPerPage();
        
        for (int row = 0; row < rowsPerColumn; row++)
        {
            Console.SetCursorPosition(0, 2 + row);
            
            for (int col = 0; col < state.ColumnCount; col++)
            {
                int entryIndex = pageStart + (col * rowsPerColumn) + row;
                
                if (entryIndex < entries.Count)
                {
                    var entry = entries[entryIndex];
                    bool isSelected = entryIndex == selectedIndex;
                    
                    RenderEntry(entry, isSelected, columnWidth);
                }
                else
                {
                    // 空のスペースを埋める
                    Console.Write(new string(' ', columnWidth));
                }
            }
        }
    }

    private static void RenderEntry(FileEntry entry, bool isSelected, int width)
    {
        // 選択状態の背景色を設定
        if (isSelected)
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ResetColor();
        }
        
        // ディレクトリとファイルで色を変える
        if (!isSelected)
        {
            if (entry.IsDirectory)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (entry.IsExecutable())
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (entry.IsTextFile())
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        
        // エントリ情報をフォーマット
        string display = FormatEntry(entry, width);
        Console.Write(display);
        
        Console.ResetColor();
    }

    private static string FormatEntry(FileEntry entry, int width)
    {
        // 名前の最大幅を計算（サイズと日付の分を引く）
        int nameWidth = width - 25; // サイズ(8) + 日付(16) + スペース(1)
        
        if (nameWidth < 10)
        {
            // 幅が狭すぎる場合は名前のみ表示
            return TruncateString(entry.Name, width);
        }
        
        string name = TruncateString(entry.Name, nameWidth);
        string size = entry.GetFormattedSize().PadLeft(8);
        string date = entry.GetFormattedDate();
        
        return $"{name} {size} {date}".PadRight(width);
    }

    private static string TruncateString(string str, int maxLength)
    {
        if (str.Length <= maxLength)
        {
            return str.PadRight(maxLength);
        }
        
        if (maxLength <= 3)
        {
            return str.Substring(0, maxLength);
        }
        
        return str.Substring(0, maxLength - 3) + "...";
    }

    private static void RenderFooter(AppState state)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        
        var selectedEntry = state.DirectoryState.GetSelectedEntry();
        int currentPage = (state.DirectoryState.PageStartIndex / state.GetTotalEntriesPerPage()) + 1;
        int totalPages = (int)Math.Ceiling((double)state.DirectoryState.Entries.Count / state.GetTotalEntriesPerPage());
        
        string footer = $" [{state.ColumnCount}列] Page {currentPage}/{totalPages} | ";
        footer += "2/3/5:列変更 Enter:開く c:コピー m:移動 d:削除 x:実行 q:終了";
        
        Console.Write(footer.PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
}

// Made with Bob
