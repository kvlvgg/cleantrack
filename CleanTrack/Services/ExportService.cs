using System.Text.Json;

namespace CleanTrack.Services
{
	public interface IExportService
	{
		public void Export(object[] objects);
	}

    public class ExportService: IExportService
	{
		public async void Export(object[] objects)
        {
			var json = JsonSerializer.Serialize(objects);
			var fileName = "cleantimer-export.json";
			var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

			File.WriteAllText(filePath, json);

			await Share.RequestAsync(new ShareFileRequest
			{
				Title = "Поделиться данными CleanTrack",
				File = new ShareFile(filePath)
			});
		}
    }
}
