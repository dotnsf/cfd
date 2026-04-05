using System.Diagnostics;

namespace CfdApp.Operations;

/// <summary>
/// 実行可能ファイルを実行
/// </summary>
public class FileExecutor
{
    /// <summary>
    /// ファイルを実行
    /// </summary>
    public static bool Execute(string filePath)
    {
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            // プロセス起動情報を設定
            var startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                WorkingDirectory = Path.GetDirectoryName(filePath) ?? Environment.CurrentDirectory,
                UseShellExecute = false
            };

            // バッチファイルやコマンドファイルの場合は cmd.exe 経由で実行
            if (extension is ".bat" or ".cmd")
            {
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/c \"{filePath}\"";
            }

            // コンソールをクリアして実行
            Console.Clear();
            Console.WriteLine($"実行中: {Path.GetFileName(filePath)}");
            Console.WriteLine(new string('─', Console.WindowWidth));
            Console.WriteLine();

            // プロセスを起動して完了を待つ
            using var process = Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit();
                
                Console.WriteLine();
                Console.WriteLine(new string('─', Console.WindowWidth));
                Console.WriteLine($"終了コード: {process.ExitCode}");
            }

            // キー入力を待つ
            Console.WriteLine();
            Console.WriteLine("何かキーを押すと戻ります...");
            Console.ReadKey(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("実行エラー:");
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("何かキーを押すと戻ります...");
            Console.ReadKey(true);
            return false;
        }
    }
}

// Made with Bob
