using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace YT_DLP_GuiWrapper;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void About_Click(object? sender, EventArgs e)
    {
        var win = (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow as MainWindow;

        if (win is not null)
            await win.ShowMessageAsync("About", "YT-DLP GuiWrapper\n\nMade by AxlRocket");
    }

    private void Logs_Click(object? sender, EventArgs e)
    {
        var win = (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
            ?.MainWindow as MainWindow;

        if (win is not null)
            win.showLogs();
    }
}