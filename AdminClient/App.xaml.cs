using System.Windows;
using System.Windows.Threading;  // This gives us access to Dispatcher
using AdminClient.Services;
using AdminClient.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syncfusion.SfSkinManager;

namespace AdminClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8 / V1NMaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH1cc3VXQ2ZcUkxxWEo =");
            SfSkinManager.ApplyStylesOnApplication = true;


            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Add appsettings.json
                    builder.AddJsonFile("appsettings.json", optional: false);

                    // Add appsettings.Development.json if it exists
                    builder.AddJsonFile($"appsettings.Development.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services, context.Configuration);
                })
                .Build();
        }

        // Dependency injection chain...
        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register configuration - this is important!
            services.AddSingleton<IConfiguration>(configuration);

            // Register HttpClient factory
            services.AddHttpClient();

            // Register our ApiService
            services.AddSingleton<ApiService>();

            // Register ViewModels 
            services.AddSingleton<MainWindowViewModel>();

            // Register MainWindow which uses the MainWindowViewModel
            services.AddSingleton<MainWindow>();


            // Register Dispatcher
            services.AddSingleton(Dispatcher.CurrentDispatcher);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // First, we need to start our host which initializes all our services
            await _host.StartAsync();

            try
            {
                // Get the main window from the dependency injection container
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();

                // Show the main window - this makes it visible to the user
                mainWindow.Show();

                // Set it as the application's main window
                MainWindow = mainWindow;
            }
            catch (Exception ex)
            {
                // Log any startup errors
                MessageBox.Show($"An error occurred during startup: {ex.Message}",
                               "Startup Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);

                // Shutdown the application if we can't start properly
                Shutdown(-1);
            }

            // Don't forget to call the base implementation
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }

        // Provide a static way to access services (useful for XAML)
        public static T GetService<T>()
            where T : class
        {
            if ((Current as App)?._host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} not found");
            }
            return service;
        }
    }

}
