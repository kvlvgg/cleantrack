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
        public Guid? ParentId { get; set; }
    }

    public class Generator
    {
        public static void HouseholdChores(ApplicationDBContext context)
        {
            context.HouseholdChores.ExecuteDelete();

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Mock");
            string householdChoresPath = Path.Combine(directoryPath, "household-chores.json");

            using (FileStream fs = File.OpenRead(householdChoresPath))
            {
                var items = JsonSerializer.Deserialize<IEnumerable<MockHouseholdChore>>(fs);

                foreach(var item in items)
                {
                    context.HouseholdChores.Add(new() { 
                        Id = item.Id,
                        Name = item.Name,
                        DayInterval = item.DayInterval,
                        LastDateDone = item.LastDateDone != null ? DateTime.Parse(item.LastDateDone) : null,
                        ParentId = item.ParentId,
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
