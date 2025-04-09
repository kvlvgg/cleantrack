using CleanTrack.Services;

namespace CleanTrack
{
	public partial class App : Application
	{
		public static IServiceProvider? Services { get; set; }

		public App()
		{
			InitializeComponent();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			return new Window(new MainPage()) { Title = "CleanTrack" };
		}

		protected override void OnResume()
		{
			base.OnResume();

			var appState = App.Services?.GetService<AppStateService>();
			appState?.NotifyStateChanged();
		}
	}
}
