using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

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
        string? path;

        switch (editor.ToLowerInvariant())
        {
            case "notepad++":
            {
                path = FindInstallLocation("Notepad++") ?? "";
                    if (!path.EndsWith("notepad++.exe"))
                    {
                        Path.Combine(path ?? "", "notepad++.exe");
                    }
                    break;
            }
            case "vscode":
            {
                path = FindInstallLocation("Visual Studio Code") ?? "";
                    if (!path.EndsWith("Code.exe"))
                    {
                        Path.Combine(path ?? "", "Code.exe");
                    }
                    break;
            }
            case "notepad":
            default:
                path = FindInstallLocation("Windows Notepad");
                if (string.IsNullOrEmpty(path)) { 
                    path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                        "notepad.exe");
                }
                if (!path.EndsWith("notepad.exe"))
                {
                    Path.Combine(path ?? "", "notepad.exe");
                }
                break;
        }

        if (string.IsNullOrEmpty(path) || !Path.Exists(path))
        {
            MessageBox.Show($"Der Installationsort des eingestellten Editors ({editor}) konnte nicht gefunden werden. Ist er nicht installiert? Der Standart Editor wird verwendet.", "NX Start Center", MessageBoxButton.OK, MessageBoxImage.Error);
            path = FindEditor("notepad");
        }

        return path;
    }

    public static string? FindInstallLocation(string appName)
    {
        string[] registryPaths =
        {
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    };

        RegistryHive[] hives =
        {
        RegistryHive.LocalMachine,
        RegistryHive.CurrentUser
    };

        foreach (var hive in hives)
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);

            foreach (var path in registryPaths)
            {
                using var uninstallKey = baseKey.OpenSubKey(path);
                if (uninstallKey == null)
                    continue;

                foreach (var subKeyName in uninstallKey.GetSubKeyNames())
                {
                    using var appKey = uninstallKey.OpenSubKey(subKeyName);
                    var displayName = appKey?.GetValue("DisplayName") as string;

                    if (displayName == null)
                        continue;

                    if (displayName.Contains(appName, StringComparison.OrdinalIgnoreCase))
                    {
                        var result =
                            appKey?.GetValue("DisplayIcon") as string
                            ?? appKey?.GetValue("InstallLocation") as string;
                        return result != null ? CleanPath(result) : null;
                    }
                }
            }
        }

        return null;
    }

    static string CleanPath(string path)
    {
        path = path.Trim('"');

        if (path.EndsWith(",0"))
            path = path[..^2];

        return path;
    }
}