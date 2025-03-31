using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using CleanTimer.Repository;
using CleanTimer.ViewModel;
using CleanTimer.Models;
using CleanTimer.Services;

namespace CleanTimer;

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
		builder.Services.AddSingleton<IHouseholdChoresViewModel, HouseholdChoresViewModel>();
		builder.Services.AddSingleton<IFormHouseholdChoresViewModel, FormHouseholdChoreViewModel>();
		builder.Services.AddSingleton<IRepository<HouseholdChore>, HouseholdChoreRepository>();

        string dataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cleantimer.db");
        builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlite($"Data Source={dataSource}"));

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		App.Services = app.Services;

		return app;
	}
}
