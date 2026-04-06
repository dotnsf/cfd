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
        
        // カラム幅を計算（カラム間のスペース1文字を考慮）
        int totalSeparators = state.ColumnCount - 1; // カラム間の区切り数
        int availableWidth = Console.WindowWidth - totalSeparators;
        int columnWidth = availableWidth / state.ColumnCount;
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
                
                // 最後のカラム以外は区切りスペースを追加
                if (col < state.ColumnCount - 1)
                {
                    Console.Write(" ");
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
            return TruncateStringByDisplayWidth(entry.Name, width);
        }
        
        string name = TruncateStringByDisplayWidth(entry.Name, nameWidth);
        string size = entry.GetFormattedSize().PadLeft(8);
        string date = entry.GetFormattedDate();
        
        string result = $"{name} {size} {date}";
        return PadToDisplayWidth(result, width);
    }

    /// <summary>
    /// 文字列の表示幅を計算（全角文字は2、半角文字は1）
    /// </summary>
    private static int GetDisplayWidth(string str)
    {
        int width = 0;
        foreach (char c in str)
        {
            // 全角文字（日本語、中国語など）は2文字分の幅
            if (c >= 0x1100 && (
                (c >= 0x1100 && c <= 0x115F) ||  // Hangul Jamo
                (c >= 0x2E80 && c <= 0x9FFF) ||  // CJK
                (c >= 0xAC00 && c <= 0xD7AF) ||  // Hangul Syllables
                (c >= 0xF900 && c <= 0xFAFF) ||  // CJK Compatibility Ideographs
                (c >= 0xFE10 && c <= 0xFE19) ||  // Vertical forms
                (c >= 0xFE30 && c <= 0xFE6F) ||  // CJK Compatibility Forms
                (c >= 0xFF00 && c <= 0xFF60) ||  // Fullwidth Forms
                (c >= 0xFFE0 && c <= 0xFFE6)))
            {
                width += 2;
            }
            else
            {
                width += 1;
            }
        }
        return width;
    }

    /// <summary>
    /// 表示幅を考慮して文字列を切り詰める
    /// </summary>
    private static string TruncateStringByDisplayWidth(string str, int maxWidth)
    {
        if (GetDisplayWidth(str) <= maxWidth)
        {
            return PadToDisplayWidth(str, maxWidth);
        }
        
        if (maxWidth <= 3)
        {
            // 幅が狭すぎる場合は単純に切り詰める
            int width = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int charWidth = (str[i] >= 0x1100 && IsWideChar(str[i])) ? 2 : 1;
                if (width + charWidth > maxWidth)
                {
                    return str.Substring(0, i);
                }
                width += charWidth;
            }
            return str;
        }
        
        // "..." を追加するスペースを確保
        int targetWidth = maxWidth - 3;
        int accumulatedWidth = 0;
        int cutIndex = 0;
        
        for (int i = 0; i < str.Length; i++)
        {
            int charWidth = (str[i] >= 0x1100 && IsWideChar(str[i])) ? 2 : 1;
            if (accumulatedWidth + charWidth > targetWidth)
            {
                break;
            }
            accumulatedWidth += charWidth;
            cutIndex = i + 1;
        }
        
        return PadToDisplayWidth(str.Substring(0, cutIndex) + "...", maxWidth);
    }

    /// <summary>
    /// 表示幅を考慮して文字列をパディング
    /// </summary>
    private static string PadToDisplayWidth(string str, int targetWidth)
    {
        int currentWidth = GetDisplayWidth(str);
        if (currentWidth >= targetWidth)
        {
            return str;
        }
        
        // 不足分をスペースで埋める
        return str + new string(' ', targetWidth - currentWidth);
    }

    /// <summary>
    /// 全角文字かどうかを判定
    /// </summary>
    private static bool IsWideChar(char c)
    {
        return (c >= 0x1100 && c <= 0x115F) ||
               (c >= 0x2E80 && c <= 0x9FFF) ||
               (c >= 0xAC00 && c <= 0xD7AF) ||
               (c >= 0xF900 && c <= 0xFAFF) ||
               (c >= 0xFE10 && c <= 0xFE19) ||
               (c >= 0xFE30 && c <= 0xFE6F) ||
               (c >= 0xFF00 && c <= 0xFF60) ||
               (c >= 0xFFE0 && c <= 0xFFE6);
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
