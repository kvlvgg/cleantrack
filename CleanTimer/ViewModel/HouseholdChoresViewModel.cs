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

        public IList<HouseholdChoreNode> Children { get; set; } = new List<HouseholdChoreNode>();
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

        public IList<HouseholdChoreNode> tree
        {
            get
            {
                HouseholdChoreNode toNode(HouseholdChore model) => new()
                {
                    Id = model.Id,
                    Name = model.Name,
                    PercentProgress = 0,
                    ParentId = model.ParentId,
                    Children = new List<HouseholdChoreNode>()
                };

                IList<HouseholdChoreNode> buildRootNodes(IEnumerable<HouseholdChore> entities) =>
                    entities.Where(x => x.ParentId == null).Select(x => toNode(x)).ToList();

                IList<HouseholdChoreNode> buildChildNodes(IEnumerable<HouseholdChore> entities, HouseholdChoreNode node) =>
                    entities.Where(x => x.ParentId == node.Id).Select(x => toNode(x)).ToList();

                IList<HouseholdChoreNode> buildTreeNodes(IEnumerable<HouseholdChore> householdChore, HouseholdChoreNode? treeNode = null)
                {
                    IList<HouseholdChoreNode> childNodes = treeNode == null ? buildRootNodes(householdChore) : buildChildNodes(householdChore, treeNode);

                    foreach (HouseholdChoreNode node in childNodes)
                    {
                        node.Children = buildTreeNodes(householdChore, node).ToList();
                    }

                    return childNodes;
                }

                return buildTreeNodes(entities);
            }
        }


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
