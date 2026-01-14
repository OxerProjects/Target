namespace Target.Models
{
    // מייצג תרגיל בודד בתוך תוכנית
    public class ExerciseDetail
    {
        public string Name { get; set; } // חובה get ו-set
        public string Description { get; set; }
        public string DurationOrReps { get; set; }
    }

    // מייצג תבנית אימון מלאה של יחידה
    public class UnitWorkoutTemplate
    {
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public List<ExerciseDetail> Exercises { get; set; } = new();
    }
}