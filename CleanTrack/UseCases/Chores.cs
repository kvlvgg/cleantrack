using CleanTrack.Models;

namespace CleanTrack.UseCases
{
	public static class Chores
	{
		public static double GetPercentSummary(double[] percents)
		{
			if (percents.Length == 0) return double.NaN;
			return percents.Aggregate(0.0, (acc, curr) => acc + curr) / percents.Length;
		}

		public static double GetProgressPercent(Chore entity)
		{
			if (!entity.isLeaf) return 0.0;

			int hourInterval = (entity.DayInterval ?? 0) * 24;
			TimeSpan diff = (entity.LastDateDone ?? DateTime.UtcNow) - DateTime.Now;
			double percentProgress = (hourInterval + diff.TotalHours) / hourInterval;

			if (percentProgress < -1) percentProgress = -1;

			return percentProgress;
		}

		public static IEnumerable<Chore> GetAllLeafs(IEnumerable<Chore> entities)
		{
			return entities.Where(x => x.isLeaf);
		}

		public static IEnumerable<Chore> GetRoots(IEnumerable<Chore> entities)
		{
			return entities.Where(x => IsRoot(x));
		}

		public static IEnumerable<Chore> GetChildren(IEnumerable<Chore> entities, INode entity)
		{
			return entities.Where(x => IsChild(entity, x));
		}

		public static IEnumerable<Chore> GetDeepChildren(IEnumerable<Chore> entities, Chore entity)
		{
			return entities.Where(x => IsDeepChild(entities, entity, x));
		}

		public static IEnumerable<Chore> GetDeepAllLeafs(IEnumerable<Chore> entities, Chore entity)
		{
			return GetChildren(entities, entity).Where(x => IsLeafOrHasLeaf(entities, x));
		}

		public static bool IsRoot(INode entity)
		{
			return entity.ParentId == null;
		}

		public static bool IsChild(INode parent, INode child)
		{
			return parent.Id == child.ParentId;
		}

		public static bool IsDeepChild(IEnumerable<Chore> entities, Chore entity, Chore target)
		{
			if (IsChild(entity, target)) return true;

			var parent = entities.FirstOrDefault(x => x.Id == target.ParentId);
			if (parent == null) return false;

			return IsDeepChild(entities, entity, parent);
		}

		public static bool IsLeafOrHasLeaf(IEnumerable<Chore> entities, Chore entity)
		{
			if (entity.isLeaf) return true;

			return GetChildren(entities, entity).Any(child => IsLeafOrHasLeaf(entities, child));
		}
	}
}
