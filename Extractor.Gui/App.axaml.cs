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
using System.IO;

namespace Extractor.Gui;

public partial class App : Application
{
    private const string ConfigFilePath = "coreConfig.json";
    private const string TracksDataFilePath = "tracksData.json";
    private const string SetupShopsDataFilePath = "setupShopsData.json";
    public static IServiceProvider Services { get; private set; } = null!;
    public static IConfiguration Configuration { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(baseDir, ConfigFilePath);
        var tracksPath = Path.Combine(baseDir, TracksDataFilePath);
        var setupShopsPath = Path.Combine(baseDir, SetupShopsDataFilePath);
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            .AddJsonFile(tracksPath, optional: false, reloadOnChange: true)
            .AddJsonFile(setupShopsPath, optional: false, reloadOnChange: true);
        Configuration = builder.Build();

        services.AddSingleton(Configuration);
        services.AddOptions<CoreConfig>().Bind(Configuration);
        services.AddOptions<TracksData>().Bind(Configuration);
        services.AddOptions<SetupShopsData>().Bind(Configuration);
        
        services.AddAutoMapper(typeof(App));

        services.AddSingleton<IWriter<CoreConfig>>(new SettingsJsonWriter(configPath));
        services.AddSingleton<IWriter<TracksData>>(new TracksJsonWriter(tracksPath));
        services.AddSingleton<IWriter<SetupShopsData>>(new SetupShopsJsonWriter(setupShopsPath));
        services.AddTransient<IArchiveHandler, ZipArchiveHandler>();
        services.AddTransient<IPathGenerator, PathGenerator>();
        services.AddTransient<IPathContextComposer, PathContextComposer>();
        services.AddTransient<IComponentMatcher<CarMatchResult>, CarMatcher>();
        services.AddTransient<IComponentMatcher<TrackMatchResult>, TrackMatcher>();
        services.AddTransient<IComponentMatcher<SetupShopMatchResult>, SetupShopMatcher>();

        services.AddTransient<IFolderPicker, FolderPicker>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<TracksViewModel>();
        services.AddTransient<SetupShopsViewModel>();
        services.AddTransient<PathTemplateBuilderViewModel>();

        Services = services.BuildServiceProvider();
            
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Services.GetService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}