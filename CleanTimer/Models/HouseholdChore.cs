using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTimer.Models;

public class HouseholdChore
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DayInterval { get; set; }
    public DateTime LastDayDone { get; set; }
    public Guid ParentId { get; set; }
}