using NXStartCenter.Model;
using NXStartCenter.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NXStartCenter.Services;


public sealed class AxelsPunktService(AppModel model, StatusViewModel status)
{
    private const string AxelsPunktProcessName = "AxelsPunkt";
    private const string AxelsPunktExeName = "AxelsPunkt.exe";

    public void ApplyAxelsPunktSetting()
    {
        try
        {
            if (model.Settings.StartAxelsPunkt)
                StartAxelsPunkt();
            else
                StopAxelsPunkt();
        }
        catch (Exception ex)
        {

            status.SetError(ex.Message);
        }
    }

    private static bool IsAxelsPunktRunning()
    {
        return Process.GetProcessesByName(AxelsPunktProcessName).Any();
    }

    private static string GetAxelsPunktExePath()
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "resources");
        return Path.Combine(folder, AxelsPunktExeName);
    }

    private void StartAxelsPunkt()
    {
        if (IsAxelsPunktRunning())
            return;

        var exePath = GetAxelsPunktExePath();

        if (!File.Exists(exePath))
        {
            throw new Exception($"AxelsPunkt.exe nicht gefunden: {exePath}");
        }

        try
        {
            ProcessService.StartFile(exePath);

        }
        catch (Exception)
        {

            throw;
        }

    }

    private void StopAxelsPunkt()
    {
        foreach (var process in Process.GetProcessesByName(AxelsPunktProcessName))
        {
            try
            {
                if (!process.CloseMainWindow())
                    process.Kill();

                if (!process.WaitForExit(3000))
                    process.Kill();
            }
            catch
            {
                // Prozess kann bereits beendet sein
            }
            finally
            {
                process.Dispose();
            }
        }
    }

}
