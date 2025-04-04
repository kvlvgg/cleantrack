using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using CleanTrack.Services;

namespace CleanTrack;

[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[]
	{
		Intent.CategoryDefault,
		Intent.CategoryBrowsable
	},
	DataMimeType = "application/json",
	DataScheme = "file",
	DataPathPattern = ".*\\.json"
)]

[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[]
	{
		Intent.CategoryDefault,
		Intent.CategoryBrowsable
	},
	DataMimeType = "application/json",
	DataScheme = "content",
	DataPathPattern = ".*\\.json"
)]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
		{
			Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#101D24"));
		}

		HandleIntent(Intent);
	}

	protected override void OnNewIntent(Intent? intent)
	{
		base.OnNewIntent(intent);
		HandleIntent(intent);
	}

	private void HandleIntent(Intent? intent)
	{
		if (intent?.Action == Intent.ActionView && intent.Data != null)
		{
			var uri = intent.Data;
			var stream = ContentResolver?.OpenInputStream(uri);

			using var reader = new StreamReader(stream);
			var json = reader.ReadToEnd();

			// Сохраняем для передачи в Blazor
			//TempStorage.ImportedJson = json;
		}
	}

	public override bool DispatchKeyEvent(KeyEvent? e)
	{
		if ((e.KeyCode == Keycode.Back) && (e.Action == KeyEventActions.Down))
		{
			var appStateService = IPlatformApplication.Current?.Services.GetService<AppStateService>();
			if (appStateService?.IsOnRootPage == true)
			{
				Platform.CurrentActivity?.MoveTaskToBack(true);
				return true; // событие обработано, дальше не идёт
			}
		}

		return base.DispatchKeyEvent(e);
	}
}

