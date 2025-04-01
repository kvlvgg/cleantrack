using CleanTrack.Models;
using CleanTrack.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System.Linq;
using System.Xml.Linq;

namespace CleanTrack.ViewModel
{

    public class HouseholdChoreNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double PercentProgress { get; set; }
        public Guid? ParentId { get; set; }
        public int Order { get; set; }
        public bool isLeaf { get; set; }
        public IList<HouseholdChoreNode> Children { get; set; } = new List<HouseholdChoreNode>();
    }

    public interface IHouseholdChoresViewModel
    {
        public IList<HouseholdChoreNode> tree { get; }
        public bool IsEditMode { get; set; }
        public IList<Guid> ToggledChores { get; set; }
        public double PercentProgressSummary { get; }
        public void Load();
        public void Add(HouseholdChore householdChore);
        public void Save();
        public void Delete(HouseholdChore householdChore);
        public void ChangeOrder(Guid? id, int step);
    }

    public class HouseholdChoresViewModel : IHouseholdChoresViewModel
    {
        [Inject]
        IRepository<HouseholdChore> repo { get; set; }

        private IEnumerable<HouseholdChore> entities = new List<HouseholdChore>();

        public bool IsEditMode { get; set; } = false;

        public IList<Guid> ToggledChores { get; set; } = new List<Guid>();

        public HouseholdChoresViewModel(IRepository<HouseholdChore> repo)
        {
            this.repo = repo;
        }

        public double PercentProgressSummary => percentsProgress.Values.Aggregate(0.0, (acc, curr) => acc + curr) / percentsProgress.Count;

        private Dictionary<Guid, double> percentsProgress
        {
            get
            {
                return entities.Aggregate(new Dictionary<Guid, double>(), (acc, curr) =>
                {
                    if (curr.DayInterval == null || curr.LastDateDone == null) return acc;

                    int hourInterval = (curr.DayInterval ?? 0) * 24;
                    TimeSpan diff = (curr.LastDateDone ?? DateTime.UtcNow) - DateTime.Now;
                    double percentProgress = (hourInterval + diff.TotalHours) / hourInterval;

                    if (percentProgress < -1) percentProgress = -1;

                    acc[curr.Id] = percentProgress;

                    return acc;
                });
            }
        }

        public IList<HouseholdChoreNode> tree
        {
            get
            {
                double calculatePercentProgress(HouseholdChore entity)
                {
                    if (entity.DayInterval != null && entity.LastDateDone != null)
                        return percentsProgress[entity.Id];

                    double[] childrenPercents = [.. entities.Where(x => x.ParentId == entity.Id).Select(x => calculatePercentProgress(x))];

                    return childrenPercents.Aggregate(0.0, (acc, curr) => acc + curr) / childrenPercents.Length;
                }

                HouseholdChoreNode toNode(HouseholdChore entity) => new()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    PercentProgress = calculatePercentProgress(entity),
                    ParentId = entity.ParentId,
                    Order = entity.Order,
                    isLeaf = entity.isLeaf,
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

                    return childNodes.OrderBy(x => x.Order).ToList();
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

        public void ChangeOrder(Guid? id, int step)
        {
            if (id == null) return;
            HouseholdChore? entity = entities.FirstOrDefault(x => x.Id == id);

            if (entity == null) return;
            HouseholdChore? swappedEntity = entities.FirstOrDefault(x => x.ParentId == entity.ParentId && x.Order == entity.Order + step);

            if (swappedEntity == null) return;

            int tempOrder = entity.Order;
            entity.Order = swappedEntity.Order;
            swappedEntity.Order = tempOrder;

            Save();
        }
    }
}
