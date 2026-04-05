namespace CfdApp.Models;

/// <summary>
/// ファイルまたはディレクトリのエントリ情報
/// </summary>
public class FileEntry
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsParentDirectory { get; set; }

    /// <summary>
    /// ファイルがテキストファイルかどうかを判定
    /// </summary>
    public bool IsTextFile()
    {
        if (IsDirectory) return false;
        
        var extension = Path.GetExtension(Name).ToLowerInvariant();
        return extension switch
        {
            ".txt" or ".md" or ".log" or ".cs" or ".csproj" or ".json" or ".xml" 
            or ".html" or ".css" or ".js" or ".ts" or ".py" or ".java" or ".cpp" 
            or ".h" or ".c" or ".sh" or ".bat" or ".cmd" or ".ps1" or ".yml" 
            or ".yaml" or ".ini" or ".cfg" or ".conf" => true,
            _ => false
        };
    }

    /// <summary>
    /// ファイルが実行可能ファイルかどうかを判定
    /// </summary>
    public bool IsExecutable()
    {
        if (IsDirectory) return false;
        
        var extension = Path.GetExtension(Name).ToLowerInvariant();
        return extension is ".exe" or ".bat" or ".cmd";
    }

    /// <summary>
    /// ファイルサイズを人間が読みやすい形式で取得
    /// </summary>
    public string GetFormattedSize()
    {
        if (IsDirectory) return "<DIR>";
        if (IsParentDirectory) return "<UP>";
        
        if (Size < 1024) return $"{Size}B";
        if (Size < 1024 * 1024) return $"{Size / 1024}KB";
        if (Size < 1024 * 1024 * 1024) return $"{Size / (1024 * 1024)}MB";
        return $"{Size / (1024 * 1024 * 1024)}GB";
    }

    /// <summary>
    /// 更新日時を表示用にフォーマット
    /// </summary>
    public string GetFormattedDate()
    {
        return LastModified.ToString("yyyy/MM/dd HH:mm");
    }
}

// Made with Bob
