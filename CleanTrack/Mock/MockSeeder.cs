#if DEBUG
using System.Text.Json;

using CleanTrack.Repository;
using Microsoft.EntityFrameworkCore;

namespace CleanTrack.Mock
{
	class MockChore
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int? DayInterval { get; set; }
		public string? LastDateDone { get; set; }
		public int Order { get; set; }
		public Guid? ParentId { get; set; }
	}

	class MockSeeder : IDataSeeder
	{
		public async void Seed(ApplicationDBContext context)
		{
			context.Chores.ExecuteDelete();

			try
			{
				using var stream = await FileSystem.OpenAppPackageFileAsync("mock/chores.json");
				using var reader = new StreamReader(stream);

				string contents = reader.ReadToEnd();

				IEnumerable<MockChore> items = JsonSerializer.Deserialize<IEnumerable<MockChore>>(contents) ?? [];

				foreach (var item in items)
				{
					context.Chores.Add(new()
					{
						Id = item.Id,
						Name = item.Name,
						DayInterval = item.DayInterval,
						LastDateDone = item.LastDateDone != null ? DateTime.Parse(item.LastDateDone) : null,
						Order = item.Order,
						ParentId = item.ParentId,
					});
				}
			}
			catch (Exception)
			{

			}

			context.SaveChanges();
		}
	}
}
#endif