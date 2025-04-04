using CleanTrack.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CleanTrack.Services
{
	public class AppStateService
	{
		private NavigationManager? navigationManager { get; set; }

		private IHouseholdChoresViewModel? viewModel { get; set; }

		public event Action? StateChanged;

		public bool IsOnRootPage => navigationManager?.Uri.EndsWith("/") == true;
		public bool IsEditMode => viewModel?.IsEditMode == true;
		public bool IsChoreSelected => viewModel?.SelectedChoreId != Guid.Empty;

		public void SetNavigationManager(NavigationManager navigationManager)
		{
			this.navigationManager = navigationManager;
		}

		public void SetViewModel(IHouseholdChoresViewModel householdChoreViewModel)
		{
			this.viewModel = householdChoreViewModel;
		}

		public void ExitEditMode()
		{
			if (viewModel == null) return;

			viewModel.IsEditMode = false;
			StateChanged?.Invoke();
		}

		public void UnselectChore()
		{
			if (viewModel == null) return;

			viewModel.SelectedChoreId = Guid.Empty;
			StateChanged?.Invoke();
		}

		public void NotifyStateChanged()
		{
			StateChanged?.Invoke();
		}

		// Нужно для доступа из статического метода
		private static AppStateService? _instance;

		public AppStateService()
		{
			_instance = this;
		}
	}
}
