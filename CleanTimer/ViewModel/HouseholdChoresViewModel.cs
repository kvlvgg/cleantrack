using CleanTimer.Models;
using CleanTimer.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System.Linq;
using System.Xml.Linq;

namespace CleanTimer.ViewModel
{

    public class HouseholdChoreNode
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double PercentProgress { get; set; }

        public Guid? ParentId { get; set; }
    }

    public interface IHouseholdChoresViewModel
    {
        public IList<HouseholdChoreNode> tree { get; }
        public void Load();
        public void Add(HouseholdChore householdChore);
        public void Save();
        public void Delete(HouseholdChore householdChore);
    }

    public class HouseholdChoresViewModel : IHouseholdChoresViewModel
    {
        [Inject]
        IRepository<HouseholdChore> repo { get; set; }

        private IEnumerable<HouseholdChore> entities = new List<HouseholdChore>();


        public HouseholdChoresViewModel(IRepository<HouseholdChore> repo)
        {
            this.repo = repo;
        }

        public IList<HouseholdChoreNode> tree => entities.Select<HouseholdChore, HouseholdChoreNode>(x =>
            new()
            {
                Id = x.Id,
                Name = x.Name,
                PercentProgress = 0,
                ParentId = x.ParentId
            }
        ).ToList();

        public void Load()
        {
            entities = repo.GetAll();
        }

        public void Add(HouseholdChore householdChore)
        {
            repo.Add(householdChore);
        }

        public void Save()
        {
            repo.Save();
        }

        public void Delete(HouseholdChore householdChore)
        {
            repo.Delete(householdChore);
        }
    }
}
