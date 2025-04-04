using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanTrack.Services
{
    public interface IImportService
    {
        void Import();
    }
    public class ImportService: IImportService
    {
		FilePickerFileType json = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
		{
			{ DevicePlatform.Android, new[] { "application/json" } },
			{ DevicePlatform.iOS,     new[] { "public.json" } },
			{ DevicePlatform.WinUI,   new[] { ".json" } },
			{ DevicePlatform.MacCatalyst, new[] { "public.json" } },
		});

		public async void Import()
        {
			var file = await FilePicker.PickAsync(new PickOptions
			{
				PickerTitle = "Выберите файл CleanTrack",
				FileTypes = json
			});

			if (file != null)
			{
				var json = await File.ReadAllTextAsync(file.FullPath);
				var importedTree = JsonSerializer.Deserialize<List<object>>(json);

				// загружаешь данные в приложение
			}
		}
    }
}
