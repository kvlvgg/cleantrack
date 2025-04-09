using System.Globalization;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using CleanTrack.Repository;
using CleanTrack.ViewModel;
using CleanTrack.Models;
using CleanTrack.Services;

#if DEBUG
using CleanTrack.Mock;
#endif

namespace CleanTrack;

public class AutoRegisterAttribute : Attribute { }

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
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<AppStateService>();
		builder.Services.AddSingleton<ModalService>();
		builder.Services.AddSingleton<IChoresViewModel, ChoresViewModel>();
		builder.Services.AddSingleton<IFormChoresViewModel, FormChoreViewModel>();
		builder.Services.AddSingleton<IRepository<Chore>, ChoreRepository>();
#if DEBUG
		builder.Services.AddSingleton<IDataSeeder, MockSeeder>();
#endif

		string dataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cleantrack.db");
		builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlite($"Data Source={dataSource}"));

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

		using var scope = builder.Services.BuildServiceProvider().CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
		var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

		seeder?.Seed(context);
#endif

		CultureInfo culture = CultureInfo.CurrentCulture;
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;

		var app = builder.Build();
		App.Services = app.Services;

		return app;
	}
}
