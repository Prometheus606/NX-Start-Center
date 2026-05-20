using NXStartCenter.Services;
using System.Diagnostics;

namespace NXStartCenter;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
#if DEBUG
        Debugger.Launch();
#endif
        ApplicationConfiguration.Initialize();
        var configPath = Path.Combine(AppContext.BaseDirectory, "data", "config.json");
        var model = AppModel.Load(configPath);
        using var form = new MainForm(model);
        Application.Run(form);
    }
}
