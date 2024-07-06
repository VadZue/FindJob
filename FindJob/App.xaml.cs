using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Windows;

namespace FindJob;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceCollection _services;

    public App()
    {
        _services = new ServiceCollection();
        ConfigureServices(_services);
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        try
        {
            services.AddDbContext<Database>(options =>
            {
                const string mySqlConnection = "Server=localhost;Database=JobFinder;Uid=root;Pwd=Ad_lol12345_aj;";
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
            });
            services.AddSingleton<HhApiClient>();
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    public async void OnStartup(object sender, StartupEventArgs e)
    {
        try
        {
            var windowsSwitcher = new WindowSwitcher(_services);
            await Migrate(windowsSwitcher.Provider);

            windowsSwitcher.Open<MainWindow>();
        }
        catch (Exception ex)
        {
            MessageBox.Show(JsonSerializer.Serialize(ex, options: new() { WriteIndented = true }));
            Environment.Exit(-12);
        }
    }

    private static async Task Migrate(IServiceProvider provider)
    {
        var db = provider.GetRequiredService<Database>();

        await db.Database.EnsureCreatedAsync().ConfigureAwait(false);
        await db.Database.MigrateAsync().ConfigureAwait(false);
    }
}

