using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
                options.UseNpgsql("Host=localhost; Database=Jobs; Username=vagin; Password=12345");
            });
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
            var provider = _services.BuildServiceProvider();
            var db = provider.GetRequiredService<Database>();

            db.Database.EnsureCreated();
            await db.Database.EnsureCreatedAsync().ConfigureAwait(false);
            await db.Database.MigrateAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

