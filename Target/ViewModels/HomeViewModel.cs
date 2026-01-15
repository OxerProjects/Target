using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Target.Helpers;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly UserService uv;
        private readonly FirebaseService firebaseService;

        // --- משתנים לסטטיסטיקה שבועית ---
        private int _weeklyCompleted;
        private int _weeklyMissed;
        private int _weeklyTotal;
        private double _weeklyProgress;

        public int WeeklyCompleted
        {
            get => _weeklyCompleted;
            set => SetProperty(ref _weeklyCompleted, value);
        }

        public int WeeklyMissed
        {
            get => _weeklyMissed;
            set => SetProperty(ref _weeklyMissed, value);
        }

        public int WeeklyTotal
        {
            get => _weeklyTotal;
            set => SetProperty(ref _weeklyTotal, value);
        }

        public double WeeklyProgress
        {
            get => _weeklyProgress;
            set => SetProperty(ref _weeklyProgress, value);
        }
        // ----------------------------------

        #region Title
        private string? title;
        public string? Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Today Events
        public ObservableCollection<Event> TodayEvents { get; } = new();

        private bool hasTodayEvents;
        public bool HasTodayEvents
        {
            get => hasTodayEvents;
            set
            {
                hasTodayEvents = value;
                OnPropertyChanged();
            }
        }

        public ICommand ViewEventCommand { get; }
        #endregion

        #region Dashboard Stats
        public ObservableCollection<PlanProgress> DashboardStats { get; } = new();
        #endregion

        #region Quiz
        public ICommand GoToQuizCommand { get; }
        #endregion

        public HomeViewModel()
        {
            uv = new UserService();
            firebaseService = new FirebaseService();

            ViewEventCommand = new Command<Event>(ViewEvent);
            GoToQuizCommand = new Command(GoToQuiz);

            InitializeTitleAsync();
            _ = RefreshDataAsync();
        }

        private async void InitializeTitleAsync()
        {
            string fullName = "לוחם";
            var email = await SecureStorage.GetAsync("userEmail");
            if (!string.IsNullOrEmpty(email))
            {
                var nameFromDb = await uv.GetUserFullNameByEmailAsync(email);
                if (!string.IsNullOrEmpty(nameFromDb)) fullName = nameFromDb;
            }

            var hour = DateTime.Now.Hour;
            string greeting = hour switch
            {
                >= 5 and < 12 => "בוקר טוב",
                >= 12 and < 15 => "צהריים טובים",
                >= 15 and < 18 => "אחר צהריים טובים",
                _ => "ערב טוב"
            };

            Title = $"שלום {fullName}, {greeting}!";
        }

        public async Task RefreshDataAsync()
        {
            string userEmail = Preferences.Default.Get("userEmail", string.Empty);
            if (string.IsNullOrEmpty(userEmail)) return;

            var allEventsDict = await firebaseService.GetAllDocumentsAsync("events");

            TodayEvents.Clear();
            DashboardStats.Clear();

            if (allEventsDict == null || !allEventsDict.Any()) return;

            var allUserEvents = new List<Event>();
            DateTime today = DateTime.Today;

            // חישוב תחילת השבוע (יום ראשון)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            // 1. המרה וסינון
            foreach (var entry in allEventsDict.Values)
            {
                if (!entry.TryGetValue("CreatorEmail", out var creatorObj)) continue;
                if (!creatorObj?.ToString().Equals(userEmail, StringComparison.OrdinalIgnoreCase) == true) continue;

                var ev = new Event
                {
                    Id = entry.ContainsKey("Id") ? entry["Id"]?.ToString() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(),
                    Title = entry["Title"]?.ToString() ?? string.Empty,
                    Description = entry["Description"]?.ToString() ?? string.Empty,
                    CreatorEmail = userEmail,
                    Date = entry.TryGetValue("Date", out var dObj) && DateTime.TryParse(dObj?.ToString(), out var dt) ? dt : DateTime.MinValue,
                    StartTime = TimeSpan.TryParse(entry["StartTime"]?.ToString(), out var st) ? st : TimeSpan.Zero,
                    EndTime = TimeSpan.TryParse(entry["EndTime"]?.ToString(), out var et) ? et : TimeSpan.Zero,
                    Type = entry["Type"]?.ToString() ?? "אחר",
                    RelatedUnit = entry.ContainsKey("RelatedUnit") ? entry["RelatedUnit"]?.ToString() ?? "" : "",
                    PlanGroupId = entry.ContainsKey("PlanGroupId") ? entry["PlanGroupId"]?.ToString() : null,
                    IsCompleted = entry.TryGetValue("IsCompleted", out var icObj) && bool.TryParse(icObj?.ToString(), out var ic) && ic
                };
                allUserEvents.Add(ev);
            }

            // 2. אירועי היום
            var todaysList = allUserEvents.Where(e => e.Date.Date == today).Take(2).ToList();
            foreach (var ev in todaysList) TodayEvents.Add(ev);
            HasTodayEvents = TodayEvents.Any();

            // 3. --- חישוב סטטיסטיקה שבועית ---
            var weeklyEvents = allUserEvents.Where(e => e.Date.Date >= startOfWeek.Date && e.Date.Date < endOfWeek.Date).ToList();

            WeeklyTotal = weeklyEvents.Count;
            WeeklyCompleted = weeklyEvents.Count(e => e.IsCompleted);
            // "פספוס" שבועי: תאריך עבר (עד אתמול) + לא בוצע
            WeeklyMissed = weeklyEvents.Count(e => !e.IsCompleted && e.Date.Date < today);

            // חישוב אחוזים (מונעים חלוקה ב-0)
            WeeklyProgress = WeeklyTotal > 0 ? (double)WeeklyCompleted / WeeklyTotal : 0;
            // --------------------------------

            // 4. סטטיסטיקה כללית לפי תוכניות
            var planGroups = allUserEvents.Where(e => !string.IsNullOrEmpty(e.PlanGroupId)).GroupBy(e => e.PlanGroupId);
            foreach (var group in planGroups)
            {
                var firstEvent = group.First();
                DashboardStats.Add(new PlanProgress
                {
                    PlanName = string.IsNullOrEmpty(firstEvent.RelatedUnit) ? "תוכנית" : firstEvent.RelatedUnit,
                    TotalWorkouts = group.Count(),
                    CompletedWorkouts = group.Count(e => e.IsCompleted),
                    MissedWorkouts = group.Count(e => !e.IsCompleted && e.Date.Date < today)
                });
            }
        }

        private async void ViewEvent(Event ev)
        {
            if (ev == null) return;
            await Shell.Current.GoToAsync(nameof(Target.Views.WorkoutDetailPage), new Dictionary<string, object> { ["WorkoutEvent"] = ev });
        }

        private async void GoToQuiz()
        {
            await Shell.Current.GoToAsync(nameof(Target.Views.Quiz));
        }
    }
}