using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Target.Models
{
    public class PlanProgress
    {
        public string PlanName { get; set; }
        public int TotalWorkouts { get; set; }
        public int CompletedWorkouts { get; set; }
        public int MissedWorkouts { get; set; } // תאריך עבר ולא בוצע
        public double ProgressPercentage => TotalWorkouts == 0 ? 0 : (double)CompletedWorkouts / TotalWorkouts;
    }
}
