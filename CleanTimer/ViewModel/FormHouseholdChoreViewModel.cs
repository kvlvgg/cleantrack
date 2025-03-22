using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CleanTimer.Models;
using CleanTimer.Repository;

namespace CleanTimer.ViewModel
{
	public enum Mode
	{
		Add,
		Edit
	}

	public enum NodeType
	{
		Node,
		Leaf,
	}

	public class FormHouseholdChore
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;
	}

	public interface IFormHouseholdChoresViewModel
	{
		public HouseholdChore Form { get; set; }
		public NodeType NodeType { get; set; }
		public Mode Mode { get; }
		public TimeSpan LastTimeDoneAgo { get; set; }
		public string LastDateDoneDisplayText { get; }
		public double ProgressPercent { get; }
		public void Create(Guid? parentId);
		public void LoadFormById(Guid id);
		public void Add();
		public void Update();
	}

	public class FormHouseholdChoreViewModel : IFormHouseholdChoresViewModel
	{
		[Inject]
		IRepository<HouseholdChore> repo { get; set; }

		public FormHouseholdChoreViewModel(IRepository<HouseholdChore> repo)
		{
			this.repo = repo;
			this.LastTimeDoneAgo = new TimeSpan();
		}

		public HouseholdChore Form { get; set; } = new HouseholdChore();

		public Mode Mode => Form.Id == Guid.Empty ? Mode.Add : Mode.Edit;

		private NodeType? _type;
		public NodeType NodeType
		{
			get
			{
				if (_type != null) return (NodeType)_type;
				return (Form.DayInterval == null && Form.LastDateDone == null) ? NodeType.Node : NodeType.Leaf;
			}

			set => _type = value;
		}

		private TimeSpan lastTimeDoneAgo;
		public TimeSpan LastTimeDoneAgo
		{
			get => lastTimeDoneAgo;
			set
			{
				Form.LastDateDone = DateTime.Now - value;
				lastTimeDoneAgo = value;
			}
		}

		public string LastDateDoneDisplayText => string.Join(" ", [(Form.LastDateDone?.ToString("ddd dd MMM yyyy") ?? string.Empty), (Form.LastDateDone?.ToString("t") ?? string.Empty)]);

		public double ProgressPercent
		{
			get
			{
				if (Form.DayInterval == null || Form.LastDateDone == null) return 0.0;

				int hourInterval = (Form.DayInterval ?? 0) * 24;
				TimeSpan diff = (Form.LastDateDone ?? DateTime.UtcNow) - DateTime.Now;
				double percentProgress = (hourInterval + diff.TotalHours) / hourInterval;

				if (percentProgress < -1) percentProgress = -1;

				return percentProgress;
			}
		}

		public void Create(Guid? parentId)
		{
			Form = new HouseholdChore() { ParentId = parentId };
			lastTimeDoneAgo = default;
		}

		public void LoadFormById(Guid id)
		{
			HouseholdChore entity = repo.GetById(id);
			if (entity == null) return;

			Form = new HouseholdChore()
			{
				Id = entity.Id,
				Name = entity.Name,
				DayInterval = entity.DayInterval,
				LastDateDone = entity.LastDateDone,
				ParentId = entity.ParentId
			};

			LastTimeDoneAgo = DateTime.Now - (Form.LastDateDone ?? DateTime.Now);
		}

		public void Add()
		{
			repo.Add(Form);
			repo.Save();
		}

		public void Update()
		{
			HouseholdChore entity = repo.GetById(Form.Id);
			if (entity == null) return;

			entity.Name = Form.Name;
			entity.DayInterval = Form.DayInterval;
			entity.LastDateDone = Form.LastDateDone;
			entity.ParentId = Form.ParentId;

			repo.Save();
		}
	}
}
