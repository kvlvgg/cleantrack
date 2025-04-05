namespace CleanTrack.Models;

public interface INode
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; }
}

public class Chore: INode
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DayInterval { get; set; }
    public DateTime? LastDateDone { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }

    public bool isLeaf => DayInterval != null && LastDateDone != null;
}