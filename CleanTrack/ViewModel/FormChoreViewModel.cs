using Microsoft.AspNetCore.Components;

using CleanTrack.Models;
using CleanTrack.Repository;

namespace CleanTrack.ViewModel
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

    public interface IFormChoresViewModel
    {
        public Chore Form { get; set; }
        public NodeType NodeType { get; set; }
        public Mode Mode { get; }
        public TimeSpan LastTimeDoneAgo { get; set; }
        public string LastDateDoneDisplayText { get; }
        public double ProgressPercent { get; }
        public void Create(NodeType type, Guid? parentId);
        public void LoadFormById(Guid id);
        public void Add();
        public void Update();
    }

    public class FormChoreViewModel : IFormChoresViewModel
	{
        [Inject]
        IRepository<Chore> repo { get; set; }

        public FormChoreViewModel(IRepository<Chore> repo)
        {
            this.repo = repo;
            this.LastTimeDoneAgo = new TimeSpan();
        }

        public Chore Form { get; set; } = new Chore();

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

        public double ProgressPercent => UseCases.Chores.GetProgressPercent(Form);

        public void Create(NodeType type, Guid? parentId)
        {
            IEnumerable<Chore> enities = repo.GetAll();

            Form = new Chore()
            {
                DayInterval = type == NodeType.Leaf ? 1 : null,
                LastDateDone = type == NodeType.Leaf ? DateTime.Now : null,
                Order = enities.Where(x => x.ParentId == parentId).Select(x => x.Order).Count() + 1,
                ParentId = parentId
            };

			if (Form.isLeaf) LastTimeDoneAgo = DateTime.Now - (Form.LastDateDone ?? DateTime.Now);
        }

        public void LoadFormById(Guid id)
        {
            Chore? entity = repo.GetById(id);
            if (entity == null) return;

            Form = new Chore()
            {
                Id = entity.Id,
                Name = entity.Name,
                DayInterval = entity.DayInterval,
                LastDateDone = entity.LastDateDone,
                ParentId = entity.ParentId
            };

            if (Form.isLeaf) LastTimeDoneAgo = DateTime.Now - (Form.LastDateDone ?? DateTime.Now);
            NodeType = Form.isLeaf ? NodeType.Leaf : NodeType.Node;
        }

        public void Add()
        {
            repo.Add(Form);
            repo.Save();
        }

        public void Update()
        {
            Chore? entity = repo.GetById(Form.Id);
            if (entity == null) return;

            entity.Name = Form.Name;
            entity.DayInterval = Form.DayInterval;
            entity.LastDateDone = Form.LastDateDone;
            entity.ParentId = Form.ParentId;

            repo.Save();
        }
    }
}
