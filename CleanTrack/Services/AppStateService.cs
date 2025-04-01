using Microsoft.JSInterop;

namespace CleanTrack.Services
{
	public class AppStateService
	{
		public event Action? StateChanged;
		public event Action<double>? ViewportHeightChanged;

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
