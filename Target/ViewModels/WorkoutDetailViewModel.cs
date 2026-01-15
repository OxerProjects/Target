using System.Collections.ObjectModel;
using System.Windows.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    [QueryProperty(nameof(WorkoutEvent), "WorkoutEvent")]
    public class WorkoutDetailViewModel : ViewModelBase
    {
        private readonly FirebaseService _firebaseService;
        private Event _workoutEvent;
        private string _pageTitle;
        private bool _isCompleted;

        public WorkoutDetailViewModel()
        {
            _firebaseService = new FirebaseService();
            Exercises = new ObservableCollection<ExerciseDisplayItem>();
            PageTitle = "פרטי אימון";

            ToggleCompletionCommand = new Command(ExecuteToggleCompletion);
            DeleteSingleWorkoutCommand = new Command(async () => await ExecuteDeleteSingleWorkout());
            DeleteEntirePlanCommand = new Command(async () => await ExecuteDeleteEntirePlan());
        }

        public ObservableCollection<ExerciseDisplayItem> Exercises { get; }

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public Event WorkoutEvent
        {
            get => _workoutEvent;
            set
            {
                if (SetProperty(ref _workoutEvent, value))
                {
                    if (_workoutEvent != null)
                    {
                        PageTitle = _workoutEvent.Title ?? "אימון";
                        _isCompleted = _workoutEvent.IsCompleted;
                        OnPropertyChanged(nameof(IsCompleted));

                        // קריאה לפונקציה המתוקנת לטעינת התרגילים
                        LoadExercises(_workoutEvent);
                    }
                }
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                    UpdateEventStatus(value);
                }
            }
        }

        public ICommand ToggleCompletionCommand { get; }
        public ICommand DeleteSingleWorkoutCommand { get; }
        public ICommand DeleteEntirePlanCommand { get; }

        private async void UpdateEventStatus(bool isDone)
        {
            if (WorkoutEvent == null) return;
            WorkoutEvent.IsCompleted = isDone;
            try { await _firebaseService.UpdateEventAsync(WorkoutEvent); }
            catch { /* טיפול בשגיאות */ }
        }

        private void ExecuteToggleCompletion() => IsCompleted = !IsCompleted;

        private async Task ExecuteDeleteSingleWorkout()
        {
            // (הקוד שלך נשאר זהה כאן)
            if (WorkoutEvent == null) return;
            bool answer = await Shell.Current.DisplayAlert("מחיקה", "למחוק את האימון הזה בלבד?", "כן, מחק", "ביטול");
            if (!answer) return;
            if (!string.IsNullOrEmpty(WorkoutEvent.Id))
            {
                await _firebaseService.DeleteDocumentAsync("events", WorkoutEvent.Id);
                await Shell.Current.GoToAsync("..");
            }
        }

        private async Task ExecuteDeleteEntirePlan()
        {
            // (הקוד שלך נשאר זהה כאן)
            if (string.IsNullOrEmpty(WorkoutEvent?.PlanGroupId))
            {
                await Shell.Current.DisplayAlert("מידע", "אימון זה אינו חלק מתוכנית.", "אישור");
                return;
            }
            bool answer = await Shell.Current.DisplayAlert("מחיקה גורפת", "למחוק את כל התוכנית?", "מחק הכל", "ביטול");
            if (!answer) return;
            await _firebaseService.DeleteEventsByGroupIdAsync(WorkoutEvent.PlanGroupId);
            await Shell.Current.GoToAsync("..");
        }

        private void LoadExercises(Event ev)
        {
            Exercises.Clear();

            // שלב 1: מנסים למצוא את הנתונים המובנים מה-Service לפי שם היחידה
            // אנחנו בודקים גם את RelatedUnit וגם את Title כדי למצוא התאמה (למשל "504")
            string unitName = ev.RelatedUnit ?? ev.Title;

            var structuredWorkout = WorkoutsDataService.GetWorkoutByUnit(unitName);

            // אם נמצא אימון מובנה במאגר - טוענים אותו!
            if (structuredWorkout != null && structuredWorkout.Exercises != null && structuredWorkout.Exercises.Count > 0)
            {
                foreach (var ex in structuredWorkout.Exercises)
                {
                    Exercises.Add(new ExerciseDisplayItem
                    {
                        Name = ex.Name,
                        Description = ex.Description,
                        DurationOrReps = ex.DurationOrReps,
                        ImageUrl = ex.ImageUrl // לוקח את ה-URL האמיתי מהדאטה
                    });
                }
                return; // סיימנו, לא צריך להמשיך ללוגיקה הישנה
            }

            // שלב 2: אם לא מצאנו במאגר (אימון מותאם אישית) - משתמשים בתיאור הטקסטואלי (Fallback)
            if (string.IsNullOrEmpty(ev.Description))
            {
                Exercises.Add(new ExerciseDisplayItem
                {
                    Name = ev.Title ?? "אימון",
                    Description = "אין פירוט תרגילים",
                    ImageUrl = "dumbbell_icon.png"
                });
            }
            else
            {
                var lines = ev.Description.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    Exercises.Add(new ExerciseDisplayItem
                    {
                        Name = line,
                        Description = "",
                        DurationOrReps = "",
                        ImageUrl = "dumbbell_icon.png" // תמונה כללית כי אין לנו פירוט
                    });
                }
            }
        }
    }

    public class ExerciseDisplayItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DurationOrReps { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "dumbbell_icon.png";
    }
}