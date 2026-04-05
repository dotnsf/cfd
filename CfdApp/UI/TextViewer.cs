using CfdApp.Models;

namespace CfdApp.UI;

/// <summary>
/// テキストファイルビューア
/// </summary>
public class TextViewer
{
    private static List<string> _lines = new();

    /// <summary>
    /// テキストファイルを読み込む
    /// </summary>
    public static bool LoadFile(string filePath)
    {
        try
        {
            _lines = File.ReadAllLines(filePath).ToList();
            return true;
        }
        catch
        {
            _lines = new List<string> { "ファイルの読み込みに失敗しました。" };
            return false;
        }
    }

    /// <summary>
    /// テキストビューアを描画
    /// </summary>
    public static void Render(AppState state)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        
        // ヘッダーを描画
        RenderHeader(state);
        
        // テキスト内容を描画
        RenderContent(state);
        
        // フッターを描画
        RenderFooter(state);
    }

    private static void RenderHeader(AppState state)
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        
        var header = $" Text Viewer - {Path.GetFileName(state.TextViewerFilePath ?? "")}";
        Console.Write(header.PadRight(Console.WindowWidth));
        
        Console.SetCursorPosition(0, 1);
        Console.ResetColor();
        Console.Write(new string('─', Console.WindowWidth));
    }

    private static void RenderContent(AppState state)
    {
        int availableLines = Console.WindowHeight - 3; // ヘッダー2行 + フッター1行
        int startLine = state.TextViewerScrollOffset;
        int endLine = Math.Min(startLine + availableLines, _lines.Count);
        
        Console.ResetColor();
        
        for (int i = startLine; i < endLine; i++)
        {
            Console.SetCursorPosition(0, 2 + (i - startLine));
            string line = _lines[i];
            
            // 行が長すぎる場合は切り詰める
            if (line.Length > Console.WindowWidth)
            {
                line = line.Substring(0, Console.WindowWidth);
            }
            
            Console.Write(line.PadRight(Console.WindowWidth));
        }
        
        // 残りの行を空白で埋める
        for (int i = endLine - startLine; i < availableLines; i++)
        {
            Console.SetCursorPosition(0, 2 + i);
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }

    private static void RenderFooter(AppState state)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        
        int currentLine = state.TextViewerScrollOffset + 1;
        int totalLines = _lines.Count;
        
        string footer = $" 行 {currentLine}/{totalLines} | ↑↓/jk:スクロール ESC:戻る";
        Console.Write(footer.PadRight(Console.WindowWidth));
        
        Console.ResetColor();
    }

    /// <summary>
    /// 上にスクロール
    /// </summary>
    public static void ScrollUp(AppState state)
    {
        if (state.TextViewerScrollOffset > 0)
        {
            state.TextViewerScrollOffset--;
        }
    }

    /// <summary>
    /// 下にスクロール
    /// </summary>
    public static void ScrollDown(AppState state)
    {
        int maxOffset = Math.Max(0, _lines.Count - (Console.WindowHeight - 3));
        if (state.TextViewerScrollOffset < maxOffset)
        {
            state.TextViewerScrollOffset++;
        }
    }

    /// <summary>
    /// ページアップ
    /// </summary>
    public static void PageUp(AppState state)
    {
        int pageSize = Console.WindowHeight - 3;
        state.TextViewerScrollOffset = Math.Max(0, state.TextViewerScrollOffset - pageSize);
    }

    /// <summary>
    /// ページダウン
    /// </summary>
    public static void PageDown(AppState state)
    {
        int pageSize = Console.WindowHeight - 3;
        int maxOffset = Math.Max(0, _lines.Count - pageSize);
        state.TextViewerScrollOffset = Math.Min(maxOffset, state.TextViewerScrollOffset + pageSize);
    }
}

// Made with Bob
