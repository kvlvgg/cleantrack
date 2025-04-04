using CleanTrack.Models;
using CleanTrack.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System.Linq;
using System.Xml.Linq;

namespace CleanTrack.ViewModel
{

	public class HouseholdChoreNode : INode
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
		public Guid? SelectedChoreId { get; set; }
		public IList<Guid> ToggledChores { get; set; }
		public double PercentProgressSummary { get; }
		public void Load();
		public void Add(HouseholdChore householdChore);
		public void Save();
		public void Delete(Guid id);
		public void ChangeOrder(Guid? id, int step);
	}

	public class HouseholdChoresViewModel : IHouseholdChoresViewModel
	{
		[Inject]
		IRepository<HouseholdChore> repo { get; set; }

		private IEnumerable<HouseholdChore> entities = new List<HouseholdChore>();

		public bool IsEditMode { get; set; } = false;

		public Guid? SelectedChoreId { get; set; } = Guid.Empty;

		public IList<Guid> ToggledChores { get; set; } = new List<Guid>();

		public HouseholdChoresViewModel(IRepository<HouseholdChore> repo)
		{
			this.repo = repo;
		}

		public double PercentProgressSummary => UseCases.Chores.GetPercentSummary(leafsProgressPercents.Values.ToArray());

		private Dictionary<Guid, double> leafsProgressPercents =>
			UseCases.Chores
				.GetAllLeafs(entities)
				.ToDictionary(
					x => x.Id,
					x => UseCases.Chores.GetProgressPercent(x)
				);

		public IList<HouseholdChoreNode> tree
		{
			get
			{
				double calculatePercentProgress(HouseholdChore entity)
				{
					if (entity.isLeaf) return leafsProgressPercents[entity.Id];

					double[] childrenPercents = [.. UseCases.Chores.GetDeepAllLeafs(entities, entity).Select(x => calculatePercentProgress(x))];

					return UseCases.Chores.GetPercentSummary(childrenPercents);
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
					UseCases.Chores.GetRoots(entities).Select(x => toNode(x)).ToList();

				IList<HouseholdChoreNode> buildChildNodes(IEnumerable<HouseholdChore> entities, HouseholdChoreNode node) =>
					UseCases.Chores.GetChildren(entities, node).Select(x => toNode(x)).ToList();

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

		public void Delete(Guid id)
		{
			HouseholdChore? entity = entities.FirstOrDefault(x => x.Id == id);
			if (entity == null) return;

			IEnumerable<HouseholdChore> deepChildren = !entity.isLeaf ? UseCases.Chores.GetDeepChildren(entities, entity) : [];
			repo.Delete(entity);

			foreach (HouseholdChore child in deepChildren)
			{
				repo.Delete(child);
			}

			repo.Save();
			Load();
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
