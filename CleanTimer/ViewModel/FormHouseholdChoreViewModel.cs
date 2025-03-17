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
    public class FormHouseholdChore
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public interface IFormHouseholdChoresViewModel
    {
        public HouseholdChore Form { get; set; }

        public double ProgressPercent { get; }
        public void LoadFormById(Guid id);
    }

    public class FormHouseholdChoreViewModel: IFormHouseholdChoresViewModel
    {
        [Inject]
        IRepository<HouseholdChore> repo { get; set; }

        public FormHouseholdChoreViewModel(IRepository<HouseholdChore> repo)
        {
            this.repo = repo;
        }

        public HouseholdChore Form { get; set; } = new HouseholdChore();

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

        public void LoadFormById(Guid id)
        {
            Form = repo.GetById(id);
        }
    }
}
