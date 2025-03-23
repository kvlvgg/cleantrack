using System.Text.Json;

using CleanTimer.Repository;
using Microsoft.EntityFrameworkCore;

namespace CleanTimer.Mock
{
    class MockHouseholdChore
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? DayInterval { get; set; }
        public string? LastDateDone { get; set; }
		public int Order { get; set; }
        public Guid? ParentId { get; set; }
    }

    public class Generator
    {
        public async static void HouseholdChores(ApplicationDBContext context)
        {
            context.HouseholdChores.ExecuteDelete();

			try
			{
				using var stream = await FileSystem.OpenAppPackageFileAsync("mock/household-chores.json");
				using var reader = new StreamReader(stream);

				string contents = reader.ReadToEnd();

				var items = JsonSerializer.Deserialize<IEnumerable<MockHouseholdChore>>(contents);

				foreach (var item in items)
				{
					context.HouseholdChores.Add(new()
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
			catch (Exception ex)
			{

			}

            context.SaveChanges();
        }
    }
}
