using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CleanTrack.Services
{
	public class AppStateService
	{
		private NavigationManager? navigationManager { get; set; }

		public event Action? StateChanged;
		public event Action<double>? ViewportHeightChanged;

		public bool IsOnRootPage => navigationManager.Uri.EndsWith("/");

		public void SetNavigationManager(NavigationManager navigationManager)
		{
			this.navigationManager = navigationManager;
		}

		public void NotifyStateChanged()
		{
			StateChanged?.Invoke();
		}

		[JSInvokable("OnViewportResize")]
		public static Task OnViewportResize(double height)
		{
			// Получить текущий экземпляр через синглтон — нужен трюк
			_instance?.ViewportHeightChanged?.Invoke(height);
			return Task.CompletedTask;
		}

		// Нужно для доступа из статического метода
		private static AppStateService? _instance;

		public AppStateService()
		{
			_instance = this;
		}
	}
}
