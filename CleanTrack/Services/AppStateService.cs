namespace CleanTrack.Services
{
	public class AppStateService
	{
		public event Action? StateChanged;

		public void NotifyStateChanged()
		{
			StateChanged?.Invoke();
		}
	}
}
