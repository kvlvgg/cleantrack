namespace CleanTimer.Models;

public class HouseholdChore
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DayInterval { get; set; }
    public DateTime? LastDateDone { get; set; }
    public Guid? ParentId { get; set; }

    public bool isLeaf => DayInterval != null && LastDateDone != null;
}