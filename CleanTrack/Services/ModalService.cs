using Microsoft.AspNetCore.Components;

namespace CleanTrack.Services
{
	public class ModalService
	{
		public RenderFragment? Current { get; private set; }
		public event Action? OnChange;

		public void Show(RenderFragment content)
		{
			Current = content;
			OnChange?.Invoke();
		}

		public void Close()
		{
			Current = null;
			OnChange?.Invoke();
		}

		public void ConfirmDelete(string title, string message, string yesText, string noText, Action onConfirm)
		{
			MarkupString msMessage = new MarkupString(message);

			Show(builder =>
			{
				builder.OpenComponent(0, typeof(Views.ConfirmDeleteModal));
				builder.AddAttribute(1, "Title", title);
				builder.AddAttribute(2, "Message", msMessage);
				builder.AddAttribute(3, "YesText", yesText);
				builder.AddAttribute(4, "NoText", noText);
				builder.AddAttribute(5, "OnConfirm", EventCallback.Factory.Create(this, () =>
				{
					onConfirm();
					Close();
				}));
				builder.AddAttribute(6, "OnCancel", EventCallback.Factory.Create(this, Close));
				builder.CloseComponent();
			});
		}
	}
}
