namespace CfdApp.Operations;

/// <summary>
/// ファイル操作（コピー、移動、削除）
/// </summary>
public class FileOperations
{
    /// <summary>
    /// ファイルまたはディレクトリをコピー
    /// </summary>
    public static bool Copy(string sourcePath, string destinationPath, bool isDirectory)
    {
        try
        {
            // 相対パスを絶対パスに変換
            if (!Path.IsPathRooted(destinationPath))
            {
                destinationPath = Path.GetFullPath(destinationPath);
            }

            if (isDirectory)
            {
                CopyDirectory(sourcePath, destinationPath);
            }
            else
            {
                // 宛先がディレクトリの場合、ファイル名を追加
                if (Directory.Exists(destinationPath))
                {
                    destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
                }
                
                // 宛先ディレクトリが存在しない場合は作成
                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                
                File.Copy(sourcePath, destinationPath, true);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリを移動
    /// </summary>
    public static bool Move(string sourcePath, string destinationPath, bool isDirectory)
    {
        try
        {
            // 相対パスを絶対パスに変換
            if (!Path.IsPathRooted(destinationPath))
            {
                destinationPath = Path.GetFullPath(destinationPath);
            }

            if (isDirectory)
            {
                // 宛先がディレクトリの場合、ソースディレクトリ名を追加
                if (Directory.Exists(destinationPath))
                {
                    destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
                }
                
                Directory.Move(sourcePath, destinationPath);
            }
            else
            {
                // 宛先がディレクトリの場合、ファイル名を追加
                if (Directory.Exists(destinationPath))
                {
                    destinationPath = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
                }
                
                // 宛先ディレクトリが存在しない場合は作成
                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                
                File.Move(sourcePath, destinationPath, true);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリを削除
    /// </summary>
    public static bool Delete(string path, bool isDirectory)
    {
        try
        {
            if (isDirectory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ディレクトリを再帰的にコピー
    /// </summary>
    private static void CopyDirectory(string sourceDir, string destDir)
    {
        // 宛先ディレクトリが存在しない場合は作成
        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        // ファイルをコピー
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        // サブディレクトリを再帰的にコピー
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }
}

// Made with Bob
