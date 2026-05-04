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
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(ConfigFilePath, optional: false, reloadOnChange: true)
            .AddJsonFile(TracksDataFilePath, optional: false, reloadOnChange: true)
            .AddJsonFile(SetupShopsDataFilePath, optional: false, reloadOnChange: true);
        Configuration = builder.Build();
        
        services.AddAutoMapper(typeof(App));

        services.AddSingleton<ISettingsRepostory>(new SettingsJsonRepository(ConfigFilePath));
        services.AddSingleton<ITracksRepository>(new TracksJsonRepository(TracksDataFilePath));
        services.AddSingleton<ISetupShopsRepository>(new SetupShopsJsonRepository(SetupShopsDataFilePath));
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