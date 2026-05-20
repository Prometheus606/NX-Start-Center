using System.Diagnostics;
using System.IO;

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

    public static void StartBatch(string batchFile, string? arguments = null, string? workingDirectory = null, bool debug = false)
    {
        ProcessStartInfo psi;

        if (debug)
        {
            psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k \"\"{batchFile}\" {arguments ?? string.Empty}\"",
                WorkingDirectory = workingDirectory ?? string.Empty,
                UseShellExecute = true,
                CreateNoWindow = false
            };
        }
        else
        {
            psi = new ProcessStartInfo
            {
                FileName = batchFile,
                Arguments = arguments ?? string.Empty,
                WorkingDirectory = workingDirectory ?? string.Empty,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        Process.Start(psi);
    }

    public static void OpenFolder(string folder) => StartFile(folder);

    public static string? FindEditor(string editor)
    {
        var user = Environment.UserName;

        string[] paths = editor.ToLowerInvariant() switch
        {
            "notepad" => new[]
            {
                @"C:\Windows\System32\notepad.exe"
            },

            "notepad++" => new[]
            {
                @"C:\Program Files\Notepad++\notepad++.exe",
                @"C:\Program Files (x86)\Notepad++\notepad++.exe"
            },

            "vscode" => new[]
            {
                Path.Combine(@"C:\Users", user, @"AppData\Local\Programs\Microsoft VS Code\Code.exe"),
                @"C:\Program Files\Microsoft VS Code\Code.exe",
                @"C:\Program Files (x86)\Microsoft VS Code\Code.exe"
            },

            _ => Array.Empty<string>()
        };

        return paths.FirstOrDefault(File.Exists);
    }
}