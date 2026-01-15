namespace Target.Services
{
    public static class WorkoutsDataService
    {
        public class ExerciseDetail
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string DurationOrReps { get; set; }
            public string ImageUrl { get; set; } // כעת יכיל URL
        }

        public class UnitWorkout
        {
            public string UnitName { get; set; }
            public List<ExerciseDetail> Exercises { get; set; }
        }

        // מאגר מורחב עם תמונות רשת
        private static readonly Dictionary<string, UnitWorkout> _workouts = new()
{
    // --- שלדג (קומנדו אווירי: דגש על ניווט מהיר, סיבולת ורגליים) ---
    { "שלדג", new UnitWorkout { UnitName = "שלדג", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "אינטרוולים בעלייה", Description = "ריצה בעלייה תלולה (ספרינט למעלה, ירידה קלה).", DurationOrReps = "10 סבבים", ImageUrl = "https://img.icons8.com/color/96/mountain.png" },
        new ExerciseDetail { Name = "Lunges (מכרעים) בהליכה", Description = "צעדי מכרע עם משקולות ידיים, לחיזוק רגליים לשטח.", DurationOrReps = "4 סטים, 20 מטר", ImageUrl = "https://img.icons8.com/color/96/leg-workout.png" },
        new ExerciseDetail { Name = "פלאנק סטטי (מנח ירי)", Description = "החזקת גוף יציב ללא תזוזה (דימוי ציין לייזר).", DurationOrReps = "3 דקות מצטבר", ImageUrl = "https://img.icons8.com/color/96/plank.png" },
        new ExerciseDetail { Name = "ריצת 2000 מהירה", Description = "מדידת זמנים בקצב תחרותי.", DurationOrReps = "מתחת ל-8 דקות", ImageUrl = "https://img.icons8.com/color/96/running-rabbit.png" }
    }}},

    // --- 669 (חילוץ והצלה: דגש על כוח פלג גוף עליון, טיפוס וסחיבה) ---
    { "669", new UnitWorkout { UnitName = "669", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "מתח עם משקל", Description = "עליות מתח מלאות עם ווסט/משקולת 10 ק\"ג.", DurationOrReps = "5 סטים, 8 חזרות", ImageUrl = "https://img.icons8.com/color/96/pullups.png" },
        new ExerciseDetail { Name = "טיפוס חבל", Description = "טיפוס חבל 6 מטר ללא רגליים (כוח ידיים בלבד).", DurationOrReps = "3 עליות מלאות", ImageUrl = "https://img.icons8.com/color/96/climbing.png" },
        new ExerciseDetail { Name = "הליכת איכר (Farmer Walk)", Description = "הליכה עם משקולות כבדות בכל יד ליציבות ליבה.", DurationOrReps = "4 סטים, 50 מטר", ImageUrl = "https://img.icons8.com/color/96/weight-lifting.png" },
        new ExerciseDetail { Name = "סחיבת פצוע (Fireman Carry)", Description = "סקוואט או הליכה עם שק חול/בן זוג על הכתפיים.", DurationOrReps = "3 סטים, 10 חזרות", ImageUrl = "https://img.icons8.com/color/96/bodybuilding.png" }
    }}},

    // --- יהל"ם (הנדסה קרבית: דגש על כוח מתפרץ, זחילות ועבודה עם ציוד כבד) ---
    { "יהל\"ם", new UnitWorkout { UnitName = "יהל\"ם", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "זחילת דוב (Bear Crawl)", Description = "הליכה על 4 ללא ברכיים על הרצפה (דימוי מנהרות).", DurationOrReps = "4 סטים, 30 מטר", ImageUrl = "https://img.icons8.com/color/96/crawling.png" },
        new ExerciseDetail { Name = "דדליפט (Deadlift)", Description = "הרמת משקל כבד מהרצפה (חיזוק גב תחתון ורגליים).", DurationOrReps = "5 סטים, 5 חזרות", ImageUrl = "https://img.icons8.com/color/96/deadlift.png" },
        new ExerciseDetail { Name = "זריקת כדור כוח", Description = "הטחת כדור כוח לרצפה או לקיר בכוח מתפרץ.", DurationOrReps = "3 סטים, 15 חזרות", ImageUrl = "https://img.icons8.com/color/96/shot-put.png" },
        new ExerciseDetail { Name = "פטיש על צמיג", Description = "מכות פטיש כבד על צמיג טרקטור.", DurationOrReps = "דקה רצופה X 3", ImageUrl = "https://img.icons8.com/color/96/hammer.png" }
    }}},

    // --- 504 (מודיעין אנושי: דגש על סיבולת שטח, הליכות ארוכות וזריזות) ---
    { "504", new UnitWorkout { UnitName = "504", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "ריצת נפח שטח", Description = "ריצה בקצב נוח בדופק אירובי (Zone 2), עדיפות לשטח כורכר.", DurationOrReps = "10 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/running--v1.png" },
        new ExerciseDetail { Name = "אימון מדרגות", Description = "עליות כפולות, ספרינטים, וקפיצות רגליים צמודות.", DurationOrReps = "15 דקות", ImageUrl = "https://img.icons8.com/color/96/stairs.png" },
        new ExerciseDetail { Name = "Burpees (סמוך קום)", Description = "חזה לרצפה, קפיצה עם מחיאת כף מעל הראש.", DurationOrReps = "50 חזרות למדידה", ImageUrl = "https://img.icons8.com/color/96/burpees.png" },
        new ExerciseDetail { Name = "תרגילי זריזות (סולם)", Description = "עבודת רגליים מהירה, כניסה ויציאה, סקיפינג.", DurationOrReps = "10 דקות", ImageUrl = "https://img.icons8.com/color/96/ladder.png" }
    }}},

    // --- רפאים (רב-ממדית: שילוב של כושר גופני וקוגניטיבי, זריזות) ---
    { "רפאים", new UnitWorkout { UnitName = "רפאים", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "ספרינט + משימה", Description = "ספרינט 50 מטר ואז ביצוע תרגיל חישובי/הרכבה (דופק גבוה).", DurationOrReps = "6 סבבים", ImageUrl = "https://img.icons8.com/color/96/brain.png" },
        new ExerciseDetail { Name = "Box Jumps", Description = "קפיצה על ארגז כוח ונחיתה יציבה.", DurationOrReps = "3 סטים, 15 חזרות", ImageUrl = "https://img.icons8.com/color/96/jump.png" },
        new ExerciseDetail { Name = "Russian Twists", Description = "כפיפות בטן עם רוטציות לצדדים (עם משקולת).", DurationOrReps = "4 סטים, 20 חזרות", ImageUrl = "https://img.icons8.com/color/96/abs.png" },
        new ExerciseDetail { Name = "ריצת שמיניות", Description = "ריצה בין קונוסים לשינויי כיוון מהירים.", DurationOrReps = "10 דקות", ImageUrl = "https://img.icons8.com/color/96/cones.png" }
    }}},

    // --- אגוז (גרילה: דגש על שטח סבוך, הסוואה וכוח רגליים) ---
    { "אגוז", new UnitWorkout { UnitName = "אגוז", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "זחילה נמוכה", Description = "זחילה איטית ושקטה למרחק (עבודה על ליבה וכתפיים).", DurationOrReps = "100 מטר", ImageUrl = "https://img.icons8.com/color/96/army.png" },
        new ExerciseDetail { Name = "Goblet Squat", Description = "סקוואט עמוק עם משקולת צמודה לחזה.", DurationOrReps = "4 סטים, 12 חזרות", ImageUrl = "https://img.icons8.com/color/96/squats.png" },
        new ExerciseDetail { Name = "ריצת שטח משתנה", Description = "ריצה ביער/חורשה עם מכשולים טבעיים.", DurationOrReps = "6 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/forest.png" },
        new ExerciseDetail { Name = "שכיבות סמיכה יהלום", Description = "ידיים צמודות (צורת יהלום) דגש על יד אחורית.", DurationOrReps = "4 סטים עד כשל", ImageUrl = "https://img.icons8.com/color/96/pushups.png" }
    }}},

    // --- דובדבן (לוחמה בשטח בנוי: קרב מגע, תגובה מהירה) ---
    { "דובדבן", new UnitWorkout { UnitName = "דובדבן", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "קרב מגע משולב", Description = "שילוב של אגרופים, בעיטות וברכיות לשק איגרוף בעצימות גבוהה.", DurationOrReps = "5 דקות X 3 סבבים", ImageUrl = "https://img.icons8.com/color/96/boxing.png" },
        new ExerciseDetail { Name = "ספרינטים קצרים", Description = "ספרינטים 20 מטר עם שינויי כיוון חדים.", DurationOrReps = "20 חזרות", ImageUrl = "https://img.icons8.com/color/96/sprint.png" },
        new ExerciseDetail { Name = "שכיבות סמיכה פליאומטריות", Description = "ניתוק ידיים מהקרקע ומחיאת כף.", DurationOrReps = "4 סטים, 12 חזרות", ImageUrl = "https://img.icons8.com/color/96/pushups.png" },
        new ExerciseDetail { Name = "הרמת צמיג (Tire Flip)", Description = "הפיכת צמיג טרקטור כבד.", DurationOrReps = "30 מטר", ImageUrl = "https://img.icons8.com/color/96/tire.png" }
    }}},

    // --- מגלן (עומק: דגש על מסעות, משקלים ודיוק) ---
    { "מגלן", new UnitWorkout { UnitName = "מגלן", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "מסע אלונקות", Description = "הליכה מהירה עם שק כבד/אלונקה.", DurationOrReps = "3 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/trekking.png" },
        new ExerciseDetail { Name = "Thrusters", Description = "שילוב של סקוואט ולחיצת כתפיים עם משקולות.", DurationOrReps = "4 סטים, 10 חזרות", ImageUrl = "https://img.icons8.com/color/96/gym.png" },
        new ExerciseDetail { Name = "פלאנק צידי", Description = "חיזוק צידי הגוף ליציבות.", DurationOrReps = "דקה בכל צד X 3", ImageUrl = "https://img.icons8.com/color/96/plank.png" },
        new ExerciseDetail { Name = "חתירה בהטיה", Description = "חיזוק שרירי הגב (Bent over row).", DurationOrReps = "4 סטים, 12 חזרות", ImageUrl = "https://img.icons8.com/color/96/rowing.png" }
    }}},

    // --- סיירת מטכ"ל (עילית: סיבולת קיצונית, מנטליות) ---
    { "סיירת מטכ\"ל", new UnitWorkout { UnitName = "סיירת מטכ\"ל", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "ריצת ניווט ארוכה", Description = "ריצה למרחק ארוך בשטח משתנה.", DurationOrReps = "12 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/map-marker.png" },
        new ExerciseDetail { Name = "Muscle Ups", Description = "עליות כוח על מתח (למתקדמים) או מתח גבוה לחזה.", DurationOrReps = "5 סטים, מקסימום חזרות", ImageUrl = "https://img.icons8.com/color/96/pullups.png" },
        new ExerciseDetail { Name = "Sit Ups (כפיפות בטן)", Description = "כפיפות בטן מלאות קלאסיות בקצב מהיר.", DurationOrReps = "100 חזרות רצוף", ImageUrl = "https://img.icons8.com/color/96/situps.png" },
        new ExerciseDetail { Name = "אימון הפוגות (HIIT)", Description = "עבודה בעצימות מקסימלית ומנוחה קצרה.", DurationOrReps = "20 דקות", ImageUrl = "https://img.icons8.com/color/96/stopwatch.png" }
    }}},

    // --- שייטת 13 (קומנדו ימי: ים, חול, צלילה) ---
    { "שייטת 13", new UnitWorkout { UnitName = "שייטת 13", Exercises = new List<ExerciseDetail> {
        new ExerciseDetail { Name = "ריצה בחול ים עמוק", Description = "ריצה בקו המים או בחול הרך, עומס גבוה על השוקיים.", DurationOrReps = "5 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/beach.png" },
        new ExerciseDetail { Name = "צלילה דינמית", Description = "צלילה למרחק מתחת למים (בבריכה בפיקוח בלבד).", DurationOrReps = "25 מטר X 4", ImageUrl = "https://img.icons8.com/color/96/diving-goggles.png" },
        new ExerciseDetail { Name = "Flutter Kicks", Description = "בעיטות מספריים בשכיבה על הגב, ידיים מתחת לישבן.", DurationOrReps = "5 דקות מצטבר", ImageUrl = "https://img.icons8.com/color/96/abs.png" },
        new ExerciseDetail { Name = "שחייה (חתירה/חזה)", Description = "שחייה רצופה בקצב בינוני-גבוה.", DurationOrReps = "1.5 ק\"מ", ImageUrl = "https://img.icons8.com/color/96/swimming.png" }
    }}}
};

        public static UnitWorkout? GetWorkoutByUnit(string unitName)
        {
            if (string.IsNullOrEmpty(unitName)) return null;
            var cleanName = unitName.Trim();
            // חיפוש חכם יותר (מכיל את המחרוזת)
            var key = _workouts.Keys.FirstOrDefault(k => cleanName.Contains(k));
            return key != null ? _workouts[key] : null;
        }
    }
}