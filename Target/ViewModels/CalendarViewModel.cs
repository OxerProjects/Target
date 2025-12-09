using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public partial class CalendarViewModel : ObservableObject
    {
        private readonly FirebaseService firebaseService;

        public ObservableCollection<CalendarDay> DaysInMonth { get; set; } = new();
        public ObservableCollection<Event> AllEventsForMonth { get; set; } = new();
        public ObservableCollection<Event> EventsForSelectedDate { get; set; } = new();

        private CalendarDay? _lastSelectedDay;

        public DateTime CurrentMonth { get; set; } = DateTime.Today;
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string IsEventPanelVisible { get; set; } = "False";
        public string CurrentMonthYear { get; set; } = DateTime.Today.ToString("MMMM yyyy");

        public IRelayCommand PreviousMonthCommand { get; }
        public IRelayCommand NextMonthCommand { get; }
        public IRelayCommand<CalendarDay> DayTappedCommand { get; }
        public IRelayCommand AddEventCommand { get; }
        public IRelayCommand<Event> ViewEventCommand { get; }

        public CalendarViewModel(FirebaseService service)
        {
            Console.WriteLine("CalendarViewModel - CTOR CALLED");

            firebaseService = service;

            PreviousMonthCommand = new RelayCommand(GoToPreviousMonth);
            NextMonthCommand = new RelayCommand(GoToNextMonth);
            DayTappedCommand = new RelayCommand<CalendarDay>(OnDayTapped);
            AddEventCommand = new RelayCommand(OnAddEvent);
            ViewEventCommand = new RelayCommand<Event>(OnViewEvent);

            LoadMonth();
        }

        private void GoToPreviousMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
            LoadMonth();
        }

        private void GoToNextMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
            LoadMonth();
        }

        private void OnDayTapped(CalendarDay? day)
        {
            if (day == null) return;

            // ביטול בחירה קודמת
            if (_lastSelectedDay != null)
                _lastSelectedDay.IsSelected = false;

            // סימון נוכחי
            day.IsSelected = true;
            _lastSelectedDay = day;

            SelectedDate = day.Date;
            IsEventPanelVisible = "True";

            Console.WriteLine(IsEventPanelVisible);

            LoadEventsForDay(day.Date);        
        }

        private async void LoadMonth()
        {
            CurrentMonthYear = CurrentMonth.ToString("MMMM yyyy");
            DaysInMonth.Clear();

            var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
            var lastDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month));

            // שבוע מתחיל בשבת (ש) לפי ה־Grid שלנו (RightToLeft)
            int daysBefore = ((int)firstDayOfMonth.DayOfWeek + 1) % 7; // 0=שבת, 1=ראשון...
            int daysAfter = 6 - ((int)lastDayOfMonth.DayOfWeek + 1) % 7;

            // ימים מהחודש הקודם
            for (int i = daysBefore; i > 0; i--)
            {
                var date = firstDayOfMonth.AddDays(-i);
                DaysInMonth.Add(new CalendarDay { Date = date, IsCurrentMonth = false });
            }

            // ימים מהחודש הנוכחי
            for (int i = 0; i < DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month); i++)
            {
                var date = firstDayOfMonth.AddDays(i);
                DaysInMonth.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = true,
                    IsToday = date.Date == DateTime.Today
                });
            }

            // ימים מהחודש הבא
            for (int i = 1; i <= daysAfter; i++)
            {
                var date = lastDayOfMonth.AddDays(i);
                DaysInMonth.Add(new CalendarDay { Date = date, IsCurrentMonth = false });
            }


            await LoadEventsForMonth();
        }
        private void LoadEventsForDay(DateTime date)
        {
            var userEmail = Preferences.Default.Get("userEmail", "");

            var eventsToday = AllEventsForMonth
                .Where(e => e.Date.Date == date.Date && e.CreatorEmail == userEmail)
                .ToList();

            EventsForSelectedDate.Clear();

            foreach (var ev in eventsToday)
                EventsForSelectedDate.Add(ev);
        }

        private async Task LoadEventsForMonth()
        {
            AllEventsForMonth.Clear();

            var allEvents = await firebaseService.GetAllDocumentsAsync("events");
            if (allEvents == null) return;

            string userEmail = Preferences.Default.Get("userEmail", "");

            foreach (var entry in allEvents.Values)
            {
                if (!entry.TryGetValue("CreatorEmail", out var creatorObj)) continue;
                if (creatorObj?.ToString() != userEmail) continue;

                if (!entry.TryGetValue("Date", out var dateObj)) continue;
                if (!DateTime.TryParse(dateObj.ToString(), out var eventDate)) continue;

                // רק אירועים שנמצאים בחודש הנוכחי
                if (eventDate.Month != CurrentMonth.Month || eventDate.Year != CurrentMonth.Year)
                    continue;

                AllEventsForMonth.Add(new Event
                {
                    Id = entry["Id"]?.ToString() ?? Guid.NewGuid().ToString(),
                    Title = entry["Title"]?.ToString() ?? "",
                    CreatorEmail = creatorObj.ToString(),
                    Date = eventDate,
                    Type = entry["Type"]?.ToString() ?? "אחר",
                    StartTime = TimeSpan.Parse(entry["StartTime"]?.ToString() ?? "00:00"),
                    EndTime = TimeSpan.Parse(entry["EndTime"]?.ToString() ?? "00:00"),
                    Description = entry["Description"]?.ToString() ?? ""
                });
            }

            // עדכון הימים ביומן
            foreach (var day in DaysInMonth)
            {
                day.HasEvent = AllEventsForMonth.Any(e => e.Date.Date == day.Date.Date);
            }
        }


        private void LoadEventsForSelectedDate()
        {
            EventsForSelectedDate.Clear();
            foreach (var day in DaysInMonth)
            {
                if (day.Date.Date == SelectedDate.Date && day.HasEvent)
                {
                    // אירועים נוספים אם רוצים, או כבר טעון ב-LoadEventsForMonth
                }
            }
        }

        private async void OnAddEvent()
        {
            await Shell.Current.GoToAsync(nameof(Target.Views.AddEventPage));
        }

        private async void OnViewEvent(Event? ev)
        {
            if (ev == null) return;
            await Shell.Current.GoToAsync(nameof(Target.Views.EventDetailPage), new Dictionary<string, object>
            {
                ["EventId"] = ev.Id
            });
        }
    }
}
