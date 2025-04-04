using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using CleanTrack.Services;

namespace CleanTrack;

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
	}

	public override bool DispatchKeyEvent(KeyEvent? e)
	{
		if ((e.KeyCode == Keycode.Back) && (e.Action == KeyEventActions.Down))
		{
			var appStateService = IPlatformApplication.Current?.Services.GetService<AppStateService>();

			if (appStateService?.IsChoreSelected == true && appStateService?.IsEditMode == true && appStateService?.IsOnRootPage == true)
			{
				appStateService.UnselectChore();
				return true; // событие обработано, дальше не идёт
			}

			if (appStateService?.IsOnRootPage == true && appStateService?.IsEditMode == true)
			{
				appStateService.ExitEditMode();
				return true; // событие обработано, дальше не идёт
			}

			if (appStateService?.IsOnRootPage == true)
			{
				Platform.CurrentActivity?.MoveTaskToBack(true);
				return true; // событие обработано, дальше не идёт
			}
		}

		return base.DispatchKeyEvent(e);
	}
}

