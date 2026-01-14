using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;
using System.Collections.ObjectModel;

namespace Target.ViewModels
{
    [QueryProperty(nameof(UnitTitle), "UnitTitle")]
    public partial class AddTrainingPlanViewModel : ObservableObject
    {
        private readonly FirebaseService _firebaseService;

        [ObservableProperty] string unitTitle;
        [ObservableProperty] DateTime startDate = DateTime.Today;
        [ObservableProperty] DateTime endDate = DateTime.Today.AddMonths(1);
        [ObservableProperty] TimeSpan startTime = new TimeSpan(17, 0, 0); // ברירת מחדל 17:00
        [ObservableProperty] TimeSpan endTime = new TimeSpan(18, 30, 0);

        // רשימת ימים לבחירה
        public ObservableCollection<DaySelection> WeekDays { get; } = new()
        {
            new DaySelection { Name = "א'", Day = DayOfWeek.Sunday },
            new DaySelection { Name = "ב'", Day = DayOfWeek.Monday },
            new DaySelection { Name = "ג'", Day = DayOfWeek.Tuesday },
            new DaySelection { Name = "ד'", Day = DayOfWeek.Wednesday },
            new DaySelection { Name = "ה'", Day = DayOfWeek.Thursday },
            new DaySelection { Name = "ו'", Day = DayOfWeek.Friday },
            new DaySelection { Name = "ש'", Day = DayOfWeek.Saturday }
        };

        public IAsyncRelayCommand CreatePlanCommand { get; }

        public AddTrainingPlanViewModel(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            CreatePlanCommand = new AsyncRelayCommand(OnCreatePlan);
        }

        private async Task OnCreatePlan()
        {
            var selectedDays = WeekDays.Where(d => d.IsSelected).Select(d => d.Day).ToList();

            if (!selectedDays.Any())
            {
                await Shell.Current.DisplayAlert("שגיאה", "אנא בחר לפחות יום אחד בשבוע", "אוקיי");
                return;
            }

            string userEmail = Preferences.Default.Get("userEmail", string.Empty);

            try
            {
                // לולאה שעוברת יום יום מתאריך ההתחלה עד הסוף
                string planGroupId = Guid.NewGuid().ToString();

                for (DateTime date = StartDate; date <= EndDate; date = date.AddDays(1))
                {
                    if (selectedDays.Contains(date.DayOfWeek))
                    {
                        var newEvent = new Event
                        {
                            Id = Guid.NewGuid().ToString(),
                            Title = $"אימון: {UnitTitle}",
                            Description = $"תוכנית אימונים עבור יחידת {UnitTitle}",
                            Type = "אימון",
                            Date = date,
                            StartTime = StartTime,
                            EndTime = EndTime,
                            CreatorEmail = userEmail,
                            Participants = new List<string> { userEmail },

                            // השדות החדשים לזיהוי האימון
                            RelatedUnit = UnitTitle,
                            PlanGroupId = planGroupId
                        };

                        // כאן התיקון - כל השדות ממופים ידנית:
                        await _firebaseService.SaveDocumentAsync("events", newEvent.Id, new Dictionary<string, object>
                        {
                            ["Id"] = newEvent.Id,
                            ["Title"] = newEvent.Title,
                            ["Description"] = newEvent.Description,
                            ["Type"] = newEvent.Type,

                            // שמירת התאריך בפורמט אחיד (שנה-חודש-יום) כדי שיהיה קל לסנן
                            ["Date"] = newEvent.Date.ToString("yyyy-MM-dd"),

                            // המרת TimeSpan ל-String פשוט (HH:mm)
                            ["StartTime"] = newEvent.StartTime.ToString(@"hh\:mm"),
                            ["EndTime"] = newEvent.EndTime.ToString(@"hh\:mm"),

                            ["CreatorEmail"] = newEvent.CreatorEmail,
                            ["Participants"] = newEvent.Participants,

                            // השדות החדשים שהוספנו
                            ["RelatedUnit"] = newEvent.RelatedUnit,
                            ["PlanGroupId"] = newEvent.PlanGroupId
                        });
                    }
                }

                await Shell.Current.DisplayAlert("הצלחה", "תוכנית האימונים נוספה ליומן בהצלחה", "מעולה");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("שגיאה", ex.Message, "אוקיי");
            }
        }
    }

    public class DaySelection : ObservableObject
    {
        public string Name { get; set; }
        public DayOfWeek Day { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }
    }
}