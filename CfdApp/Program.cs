using CfdApp.Models;
using CfdApp.UI;
using CfdApp.Input;

namespace CfdApp;

class Program
{
    static void Main(string[] args)
    {
        // コンソール設定
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        // アプリケーション状態を初期化
        var state = new AppState();
        
        // 起動ディレクトリを読み込む
        string startPath = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
        state.DirectoryState.LoadDirectory(startPath);

        // メインループ
        while (!state.ShouldExit)
        {
            try
            {
                // 画面を描画
                RenderScreen(state);

                // キー入力を待つ
                var keyInfo = Console.ReadKey(true);

                // キー入力を処理
                KeyHandler.HandleKey(state, keyInfo);
            }
            catch (Exception ex)
            {
                // エラーが発生した場合は表示して続行
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("エラーが発生しました:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("何かキーを押すと続行します...");
                Console.ReadKey(true);
            }
        }

        // 終了時にコンソールをクリア
        Console.Clear();
        Console.CursorVisible = true;
        Console.WriteLine("CFD を終了しました。");
    }

    static void RenderScreen(AppState state)
    {
        switch (state.CurrentScreen)
        {
            case ScreenMode.FileBrowser:
                FileListRenderer.Render(state);
                break;

            case ScreenMode.TextViewer:
                TextViewer.Render(state);
                break;

            case ScreenMode.Dialog:
                DialogRenderer.Render(state);
                break;
        }
    }
}

// Made with Bob
