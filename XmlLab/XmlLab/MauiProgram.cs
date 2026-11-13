using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace XmlLab;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<ViewModels.MainViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<ViewModels.MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if WINDOWS
        builder.ConfigureLifecycleEvents(events => events.AddWindows(w => w.OnWindowCreated(window =>
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
            appWindow.Closing += async (s, e) =>
            {
                if (Services.ExitGuard.Confirmed) return;
                e.Cancel = true;                       // хрестик теж питає підтвердження
                if (await Services.ExitGuard.AskAsync())
                {
                    Services.ExitGuard.Confirmed = true;
                    Application.Current?.Quit();
                }
            };
        })));
#endif
        return builder.Build();
    }
}