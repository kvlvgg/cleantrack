using CleanTrack.Models;
using CleanTrack.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using System.Linq;
using System.Xml.Linq;

namespace CleanTrack.ViewModel
{

	public class ChoreNode : INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public double PercentProgress { get; set; }
		public Guid? ParentId { get; set; }
		public int Order { get; set; }
		public bool isLeaf { get; set; }
		public IList<ChoreNode> Children { get; set; } = new List<ChoreNode>();
	}

	public interface IChoresViewModel
	{
		public IList<ChoreNode> tree { get; }
		public bool IsEditMode { get; set; }
		public Guid? SelectedChoreId { get; set; }
		public IList<Guid> ToggledChores { get; set; }
		public double PercentProgressSummary { get; }
		public void Load();
		public void Add(Chore chore);
		public void Save();
		public void Delete(Guid id);
		public void ChangeOrder(Guid? id, int step);
	}

	public class ChoresViewModel : IChoresViewModel
	{
		[Inject]
		IRepository<Chore> repo { get; set; }

		private IEnumerable<Chore> entities = new List<Chore>();

		public bool IsEditMode { get; set; } = false;

		public Guid? SelectedChoreId { get; set; } = Guid.Empty;

		public IList<Guid> ToggledChores { get; set; } = new List<Guid>();

		public ChoresViewModel(IRepository<Chore> repo)
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

		public IList<ChoreNode> tree
		{
			get
			{
				double calculatePercentProgress(Chore entity)
				{
					if (entity.isLeaf) return leafsProgressPercents[entity.Id];

					double[] childrenPercents = [.. UseCases.Chores.GetDeepAllLeafs(entities, entity).Select(x => calculatePercentProgress(x))];

					return UseCases.Chores.GetPercentSummary(childrenPercents);
				}

				ChoreNode toNode(Chore entity) => new()
				{
					Id = entity.Id,
					Name = entity.Name,
					PercentProgress = calculatePercentProgress(entity),
					ParentId = entity.ParentId,
					Order = entity.Order,
					isLeaf = entity.isLeaf,
					Children = new List<ChoreNode>()
				};

				IList<ChoreNode> buildRootNodes(IEnumerable<Chore> entities) =>
					UseCases.Chores.GetRoots(entities).Select(x => toNode(x)).ToList();

				IList<ChoreNode> buildChildNodes(IEnumerable<Chore> entities, ChoreNode node) =>
					UseCases.Chores.GetChildren(entities, node).Select(x => toNode(x)).ToList();

				IList<ChoreNode> buildTreeNodes(IEnumerable<Chore> chore, ChoreNode? treeNode = null)
				{
					IList<ChoreNode> childNodes = treeNode == null ? buildRootNodes(chore) : buildChildNodes(chore, treeNode);

					foreach (ChoreNode node in childNodes)
					{
						node.Children = buildTreeNodes(chore, node).ToList();
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

		public void Add(Chore chore)
		{
			repo.Add(chore);
		}

		public void Save()
		{
			repo.Save();
		}

		public void Delete(Guid id)
		{
			Chore? entity = entities.FirstOrDefault(x => x.Id == id);
			if (entity == null) return;

			IEnumerable<Chore> deepChildren = !entity.isLeaf ? UseCases.Chores.GetDeepChildren(entities, entity) : [];
			repo.Delete(entity);

			foreach (Chore child in deepChildren)
			{
				repo.Delete(child);
			}

			repo.Save();
			Load();
		}

		public void ChangeOrder(Guid? id, int step)
		{
			if (id == null) return;
			Chore? entity = entities.FirstOrDefault(x => x.Id == id);

			if (entity == null) return;
			Chore? swappedEntity = entities.FirstOrDefault(x => x.ParentId == entity.ParentId && x.Order == entity.Order + step);

			if (swappedEntity == null) return;

			int tempOrder = entity.Order;
			entity.Order = swappedEntity.Order;
			swappedEntity.Order = tempOrder;

			Save();
		}
	}
}
