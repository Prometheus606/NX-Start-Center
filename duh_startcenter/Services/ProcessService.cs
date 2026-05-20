using System.Diagnostics;

namespace NXStartCenter.Services;

public static class ProcessService
{
    public static void StartFile(string file, string? arguments = null, string? workingDirectory = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = arguments ?? string.Empty,
            WorkingDirectory = workingDirectory ?? string.Empty,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    public static void OpenFolder(string folder) => StartFile(folder);

    public static string? FindEditor(string editor)
    {
        var user = Environment.UserName;
        var paths = editor.ToLowerInvariant() switch
        {
            "notepad" => [@"C:\Windows\System32\notepad.exe"],
            "notepad++" => [@"C:\Program Files\Notepad++\notepad++.exe", @"C:\Program Files (x86)\Notepad++\notepad++.exe"],
            "vscode" => [Path.Combine(@"C:\Users", user, @"AppData\Local\Programs\Microsoft VS Code\Code.exe")],
            _ => Array.Empty<string>()
        };
        return paths.FirstOrDefault(File.Exists);
    }
}
