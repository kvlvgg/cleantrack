using CleanTrack.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CleanTrack.Services
{
	public class AppStateService
	{
		private NavigationManager? navigationManager { get; set; }

		private IChoresViewModel? viewModel { get; set; }

		private ModalService? modalService { get; set; }

		public event Action? StateChanged;

		public bool IsOnRootPage => navigationManager?.Uri.EndsWith("/") == true;
		public bool IsEditMode => viewModel?.IsEditMode == true;
		public bool IsChoreSelected => viewModel?.SelectedChoreId != Guid.Empty;
		public bool IsModalOpened => modalService?.Current != null;

		public void SetNavigationManager(NavigationManager navigationManager)
		{
			this.navigationManager = navigationManager;
		}

		public void SetViewModel(IChoresViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		public void SetModalService(ModalService modalService)
		{
			this.modalService = modalService;
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

		public void CloseModal()
		{
			if (modalService == null) return;

			modalService.Close();
		}

		public void NotifyStateChanged()
		{
			StateChanged?.Invoke();
		}
	}
}
