using System;
using System.IO;
using Extractor.Core.Model;
using Extractor.Core.Services;
using Extractor.Core.Services.Interfaces;
using Extractor.Gui.Services;
using Extractor.Gui.Services.Interfaces;
using Extractor.Gui.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShadUI;

namespace Extractor.Gui;

public static class ServiceCollectionExtensions
{
    private const string ConfigFilePath = "coreConfig.json";
    private const string TracksDataFilePath = "tracksData.json";
    private const string SetupShopsDataFilePath = "setupShopsData.json";
    
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfiguration()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, ConfigFilePath);
            var tracksPath = Path.Combine(baseDir, TracksDataFilePath);
            var setupShopsPath = Path.Combine(baseDir, SetupShopsDataFilePath);
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .AddJsonFile(tracksPath, optional: false, reloadOnChange: true)
                .AddJsonFile(setupShopsPath, optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            services.AddSingleton(configuration);
            services.AddOptions<CoreConfig>().Bind(configuration);
            services.AddOptions<TracksData>().Bind(configuration);
            services.AddOptions<SetupShopsData>().Bind(configuration);

            return services;
        }
        
        public IServiceCollection RegisterCoreServices()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, ConfigFilePath);
            var tracksPath = Path.Combine(baseDir, TracksDataFilePath);
            var setupShopsPath = Path.Combine(baseDir, SetupShopsDataFilePath);
            
            services.AddSingleton<IWriter<CoreConfig>>(new SettingsJsonWriter(configPath));
            services.AddSingleton<IWriter<TracksData>>(new TracksJsonWriter(tracksPath));
            services.AddSingleton<IWriter<SetupShopsData>>(new SetupShopsJsonWriter(setupShopsPath));
            services.AddTransient<IArchiveHandler, ZipArchiveHandler>();
            services.AddTransient<IPathComposer, PathComposer>();
            services.AddTransient<IPathContextComposer, PathContextComposer>();
            services.AddTransient<IComponentMatcher<CarMatchResult>, CarMatcher>();
            services.AddTransient<IComponentMatcher<TrackMatchResult>, TrackMatcher>();
            services.AddTransient<IComponentMatcher<SetupShopMatchResult>, SetupShopMatcher>();
            
            return services;
        }

        public IServiceCollection RegisterGuiServices()
        {
            services.AddSingleton<PageManager>();
            services.AddSingleton<DialogManager>();
            services.AddSingleton<ToastManager>();
            services.AddSingleton<IExceptionHandler, ExceptionHandler>();
            services.AddKeyedTransient<IFilePicker, FolderPicker>("folder");
            services.AddKeyedTransient<IFilePicker, ZipFilePicker>("zip");
            services.AddTransient<TempDataJsonRepository>();

            return services;
        }

        public IServiceCollection RegisterViewModels()
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<TracksViewModel>();
            services.AddTransient<SetupShopsViewModel>();
            services.AddTransient<PathTemplateBuilderViewModel>();

            return services;
        }
    }
}