using Target.Models;

namespace Target.Services
{
    public static class WorkoutsDataService
    {
        public class ExerciseDetail
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? DurationOrReps { get; set; }
            public string? ImageUrl { get; set; }
        }

        public class UnitWorkout
        {
            public string? UnitName { get; set; }
            public List<ExerciseDetail>? Exercises { get; set; }
        }

        private static readonly Dictionary<string, UnitWorkout> _workouts = new()
        {
            { "504", new UnitWorkout { UnitName = "504", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "ריצת נפח", Description = "ריצה ארוכה (סיבולת)", DurationOrReps = "8 ק\"מ", ImageUrl = "run_icon.png" },
                new ExerciseDetail { Name = "סולם זריזות", Description = "קוארדינציה", DurationOrReps = "10 דקות", ImageUrl = "agility_icon.png" },
                new ExerciseDetail { Name = "Burpees", Description = "סמוך קום", DurationOrReps = "50 חזרות", ImageUrl = "burpees_icon.png" }
            }}},
            { "669", new UnitWorkout { UnitName = "669", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "מתח עם משקל", Description = "הכנה לטיפוס חבלים", DurationOrReps = "4 סטים של 8", ImageUrl = "pullup_icon.png" },
                new ExerciseDetail { Name = "הליכת איכר", Description = "סחיבת משקולות (אלונקה)", DurationOrReps = "4x50 מטר", ImageUrl = "carry_icon.png" },
                new ExerciseDetail { Name = "אינטרוולים", Description = "ריצה מהירה להתאוששות", DurationOrReps = "20 דקות", ImageUrl = "run_icon.png" }
            }}},
            { "דובדבן", new UnitWorkout { UnitName = "דובדבן", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "HIIT קרב מגע", Description = "אגרופים ובעיטות עם ספרינט", DurationOrReps = "20 דקות", ImageUrl = "mma_icon.png" },
                new ExerciseDetail { Name = "שכיבות סמיכה מוחא כף", Description = "כוח מתפרץ", DurationOrReps = "3 סטים של 10", ImageUrl = "pushup_icon.png" },
                new ExerciseDetail { Name = "ספרינטים קצרים", Description = "תגובה מהירה", DurationOrReps = "10x20 מטר", ImageUrl = "run_icon.png" }
            }}},
            { "אגוז", new UnitWorkout { UnitName = "אגוז", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "זחילות", Description = "זחילה נמוכה בחול", DurationOrReps = "50 מטר X 3", ImageUrl = "crawl_icon.png" },
                new ExerciseDetail { Name = "טיפוס הרים", Description = "Mountain Climbers", DurationOrReps = "3 סטים של דקה", ImageUrl = "climb_icon.png" },
                new ExerciseDetail { Name = "מתח רחב", Description = "חיזוק גב עליון", DurationOrReps = "MAX X 3", ImageUrl = "pullup_icon.png" }
            }}},
            { "מגלן", new UnitWorkout { UnitName = "מגלן", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "מסע אלונקות", Description = "הליכה עם משקל גב", DurationOrReps = "3 ק\"מ", ImageUrl = "carry_icon.png" },
                new ExerciseDetail { Name = "סקוואט עם משקל", Description = "חיזוק רגליים", DurationOrReps = "4 סטים של 12", ImageUrl = "squat_icon.png" },
                new ExerciseDetail { Name = "כפיפות בטן", Description = "חיזוק ליבה", DurationOrReps = "100 חזרות", ImageUrl = "abs_icon.png" }
            }}},
            { "סיירת מטכ\"ל", new UnitWorkout { UnitName = "סיירת מטכ\"ל", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "ריצת 2000", Description = "מדידת זמנים", DurationOrReps = "מדידה", ImageUrl = "run_icon.png" },
                new ExerciseDetail { Name = "טיפוס חבל", Description = "כוח ידיים וגב", DurationOrReps = "3 עליות", ImageUrl = "rope_icon.png" },
                new ExerciseDetail { Name = "מקבילים", Description = "Dips משקל גוף", DurationOrReps = "4 סטים של 15", ImageUrl = "dips_icon.png" }
            }}},
            { "רפאים", new UnitWorkout { UnitName = "רפאים", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "ריצת זריזות", Description = "שינויי כיוון מהירים", DurationOrReps = "15 דקות", ImageUrl = "run_icon.png" },
                new ExerciseDetail { Name = "קפיצות ארגז", Description = "Box Jumps", DurationOrReps = "3 סטים של 15", ImageUrl = "box_icon.png" },
                new ExerciseDetail { Name = "Lunges", Description = "מכרעים בהליכה", DurationOrReps = "3x20 מטר", ImageUrl = "leg_icon.png" }
            }}},
            { "שלדג", new UnitWorkout { UnitName = "שלדג", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "ריצת ניווט", Description = "ריצת שטח בקצב משתנה", DurationOrReps = "5 ק\"מ", ImageUrl = "map_icon.png" },
                new ExerciseDetail { Name = "ספרינטים", Description = "ספרינטים בעלייה", DurationOrReps = "10x100 מטר", ImageUrl = "run_icon.png" },
                new ExerciseDetail { Name = "סחיבת שק", Description = "הליכה עם משקל", DurationOrReps = "1 ק\"מ", ImageUrl = "carry_icon.png" }
            }}},
            { "שייטת 13", new UnitWorkout { UnitName = "שייטת 13", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "שחייה", Description = "חתירה או חזה", DurationOrReps = "1 ק\"מ", ImageUrl = "swim_icon.png" },
                new ExerciseDetail { Name = "מספריים", Description = "Flutter Kicks בשכיבה", DurationOrReps = "3 דקות", ImageUrl = "abs_icon.png" },
                new ExerciseDetail { Name = "ריצה בחול ים", Description = "משטח לא יציב", DurationOrReps = "3 ק\"מ", ImageUrl = "run_icon.png" }
            }}},
            { "יהל\"ם", new UnitWorkout { UnitName = "יהל\"ם", Exercises = new List<ExerciseDetail> {
                new ExerciseDetail { Name = "דדליפט", Description = "הרמת משקל כבד", DurationOrReps = "5 סטים של 5", ImageUrl = "deadlift_icon.png" },
                new ExerciseDetail { Name = "פלאנק", Description = "חיזוק ליבה סטטי", DurationOrReps = "3x1.5 דקות", ImageUrl = "plank_icon.png" },
                new ExerciseDetail { Name = "שכיבות סמיכה יהלום", Description = "ידיים צמודות", DurationOrReps = "3 סטים של 15", ImageUrl = "pushup_icon.png" }
            }}}
        };

        public static UnitWorkout? GetWorkoutByUnit(string unitName)
        {
            if (string.IsNullOrEmpty(unitName)) return null;
            // ניקוי שם היחידה מרווחים מיותרים במידה ויש
            var cleanName = unitName.Trim();
            return _workouts.ContainsKey(cleanName) ? _workouts[cleanName] : null;
        }
    }
}