using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Extractor.Core.Model;
using Extractor.Core.Services;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Services;
using Extractor.Gui.Services.Interfaces;
using Extractor.Gui.ViewModels;
using Extractor.Gui.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Extractor.Gui.Mappings;
using System.IO;
using System.Threading.Tasks;

namespace Extractor.Gui;

public partial class App : Application
{
    public static IServiceProvider Services { get; set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = new ServiceCollection()
            .AddConfiguration()
            .AddLogging()
            .AddAutoMapper(cfg => { cfg.AddProfile<ConfigProfile>(); })
            .RegisterCoreServices()
            .RegisterGuiServices()
            .RegisterViewModels()
            .BuildServiceProvider();

        RegisterGlobalExceptionHandlers();

        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;
        
        var mainWindowViewModel = Services.GetService<MainWindowViewModel>();
        desktop.MainWindow = new MainWindow
        {
            DataContext = mainWindowViewModel
        };

        base.OnFrameworkInitializationCompleted();
    }

    private static void RegisterGlobalExceptionHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                _ = ShowUnhandledExceptionAsync(exception, "Unhandled application exception.");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            args.SetObserved();
            _ = ShowUnhandledExceptionAsync(args.Exception, "Unobserved task exception.");
        };
    }

    private static Task ShowUnhandledExceptionAsync(Exception exception, string message)
    {
        var handler = Services.GetService<IExceptionHandler>();
        return handler?.ShowAsync(exception, "Application Error", message) ?? Task.CompletedTask;
    }
}
