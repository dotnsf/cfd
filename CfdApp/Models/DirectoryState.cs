namespace CfdApp.Models;

/// <summary>
/// 現在のディレクトリの状態を管理
/// </summary>
public class DirectoryState
{
    public string CurrentPath { get; set; } = string.Empty;
    public List<FileEntry> Entries { get; set; } = new();
    public int SelectedIndex { get; set; } = 0;
    public int PageStartIndex { get; set; } = 0;

    /// <summary>
    /// ディレクトリを読み込む
    /// </summary>
    public void LoadDirectory(string path)
    {
        CurrentPath = Path.GetFullPath(path);
        Entries.Clear();
        SelectedIndex = 0;
        PageStartIndex = 0;

        try
        {
            // 親ディレクトリを追加
            var parentDir = Directory.GetParent(CurrentPath);
            if (parentDir != null)
            {
                Entries.Add(new FileEntry
                {
                    Name = "..",
                    FullPath = parentDir.FullName,
                    IsDirectory = true,
                    IsParentDirectory = true,
                    LastModified = Directory.GetLastWriteTime(parentDir.FullName)
                });
            }

            // ディレクトリを追加
            var directories = Directory.GetDirectories(CurrentPath)
                .Select(d => new DirectoryInfo(d))
                .OrderBy(d => d.Name)
                .Select(d => new FileEntry
                {
                    Name = d.Name,
                    FullPath = d.FullName,
                    IsDirectory = true,
                    LastModified = d.LastWriteTime
                });

            Entries.AddRange(directories);

            // ファイルを追加
            var files = Directory.GetFiles(CurrentPath)
                .Select(f => new FileInfo(f))
                .OrderBy(f => f.Name)
                .Select(f => new FileEntry
                {
                    Name = f.Name,
                    FullPath = f.FullName,
                    IsDirectory = false,
                    Size = f.Length,
                    LastModified = f.LastWriteTime
                });

            Entries.AddRange(files);
        }
        catch (UnauthorizedAccessException)
        {
            // アクセス権限がない場合は空のリストを表示
        }
        catch (Exception)
        {
            // その他のエラーも無視
        }
    }

    /// <summary>
    /// 選択されているエントリを取得
    /// </summary>
    public FileEntry? GetSelectedEntry()
    {
        if (SelectedIndex >= 0 && SelectedIndex < Entries.Count)
        {
            return Entries[SelectedIndex];
        }
        return null;
    }

    /// <summary>
    /// 選択を上に移動
    /// </summary>
    public void MoveUp()
    {
        if (SelectedIndex > 0)
        {
            SelectedIndex--;
        }
    }

    /// <summary>
    /// 選択を下に移動
    /// </summary>
    public void MoveDown()
    {
        if (SelectedIndex < Entries.Count - 1)
        {
            SelectedIndex++;
        }
    }

    /// <summary>
    /// ディレクトリに入る
    /// </summary>
    public void EnterDirectory()
    {
        var selected = GetSelectedEntry();
        if (selected != null && selected.IsDirectory)
        {
            LoadDirectory(selected.FullPath);
        }
    }
}

// Made with Bob
