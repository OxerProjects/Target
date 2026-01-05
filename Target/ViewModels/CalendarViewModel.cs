using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using Target.Models;
using Target.Services;
using System.Collections.Generic;
using Target.Views;

namespace Target.ViewModels
{
    public partial class CalendarViewModel : ObservableObject
    {
        private readonly FirebaseService firebaseService;

        public ObservableCollection<CalendarDay> DaysInMonth { get; } = new();
        public ObservableCollection<Event> AllEventsForMonth { get; } = new();
        public ObservableCollection<Event> EventsForSelectedDate { get; } = new();

        private CalendarDay? _lastSelectedDay;
        private bool _isLoadingEvents = false;

        private DateTime _currentMonth = DateTime.Today;
        public DateTime CurrentMonth
        {
            get => _currentMonth;
            set
            {
                if (SetProperty(ref _currentMonth, value))
                {
                    CurrentMonthYear = _currentMonth.ToString("MMMM yyyy");
                }
            }
        }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    // תטען אירועים עבור התאריך החדש
                    _ = LoadEventsForDayAsync(_selectedDate);
                }
            }
        }

        private bool _isEventPanelVisible = false;
        public bool IsEventPanelVisible
        {
            get => _isEventPanelVisible;
            set => SetProperty(ref _isEventPanelVisible, value);
        }

        private string _currentMonthYear;
        public string CurrentMonthYear
        {
            get => _currentMonthYear;
            set => SetProperty(ref _currentMonthYear, value);
        }

        // Commands
        public IRelayCommand PreviousMonthCommand { get; }
        public IRelayCommand NextMonthCommand { get; }
        public IRelayCommand<CalendarDay> DayTappedCommand { get; }
        public IRelayCommand AddEventCommand { get; }
        public IRelayCommand<Event> ViewEventCommand { get; }

        public CalendarViewModel(FirebaseService service)
        {
            firebaseService = service ?? throw new ArgumentNullException(nameof(service));

            PreviousMonthCommand = new RelayCommand(GoToPreviousMonth);
            NextMonthCommand = new RelayCommand(GoToNextMonth);
            DayTappedCommand = new RelayCommand<CalendarDay>(OnDayTapped);
            AddEventCommand = new RelayCommand(OnAddEvent);
            ViewEventCommand = new RelayCommand<Event>(OnViewEvent);

            CurrentMonthYear = CurrentMonth.ToString("MMMM yyyy");

            // התחל טעינה אסינכרונית (constructor לא יכול להיות async)
            _ = LoadMonthAsync();
        }

        private void GoToPreviousMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(-1);
            _ = LoadMonthAsync();
        }

        private void GoToNextMonth()
        {
            CurrentMonth = CurrentMonth.AddMonths(1);
            _ = LoadMonthAsync();
        }

        private async void OnDayTapped(CalendarDay? day)
        {
            if (day == null) return;

            // ביטול בחירה קודמת
            if (_lastSelectedDay != null)
                _lastSelectedDay.IsSelected = false;

            // סימון נוכחי
            day.IsSelected = true;
            _lastSelectedDay = day;

            SelectedDate = day.Date;
            IsEventPanelVisible = true;

            await LoadEventsForDayAsync(SelectedDate);
        }

        private Task LoadMonthAsync()
        {
            CurrentMonthYear = CurrentMonth.ToString("MMMM yyyy");
            DaysInMonth.Clear();

            var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
            var lastDayOfMonth = new DateTime(
                CurrentMonth.Year,
                CurrentMonth.Month,
                DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month));

            int daysBefore = ((int)firstDayOfMonth.DayOfWeek + 1) % 7;
            int daysAfter = 6 - ((int)lastDayOfMonth.DayOfWeek + 1) % 7;

            // חודש קודם
            for (int i = daysBefore; i > 0; i--)
            {
                var date = firstDayOfMonth.AddDays(-i);
                DaysInMonth.Add(new CalendarDay { Date = date, IsCurrentMonth = false });
            }

            // חודש נוכחי
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

            // חודש הבא
            for (int i = 1; i <= daysAfter; i++)
            {
                var date = lastDayOfMonth.AddDays(i);
                DaysInMonth.Add(new CalendarDay { Date = date, IsCurrentMonth = false });
            }

            return Task.CompletedTask;
        }

        private async Task LoadEventsForDayAsync(DateTime date)
        {
            if (_isLoadingEvents)
                return;

            _isLoadingEvents = true;

            try
            {
                EventsForSelectedDate.Clear();

                string userEmail = Preferences.Default.Get("userEmail", string.Empty);
                var allEvents = await firebaseService.GetAllDocumentsAsync("events");
                if (allEvents == null) return;

                foreach (var entry in allEvents.Values)
                {
                    if (!entry.TryGetValue("CreatorEmail", out var creatorObj)) continue;
                    if (!creatorObj?.ToString()
                        .Equals(userEmail, StringComparison.OrdinalIgnoreCase) == true)
                        continue;

                    if (!entry.TryGetValue("Date", out var dateObj)) continue;
                    if (!DateTime.TryParse(dateObj?.ToString(), out var eventDate)) continue;

                    if (eventDate.Date != date.Date) continue;

                    var ev = new Event
                    {
                        Id = entry.ContainsKey("Id")
                            ? entry["Id"]?.ToString() ?? Guid.NewGuid().ToString()
                            : Guid.NewGuid().ToString(),
                        Title = entry["Title"]?.ToString() ?? string.Empty,
                        CreatorEmail = userEmail,
                        Date = eventDate,
                        Type = entry["Type"]?.ToString() ?? "אחר",
                        StartTime = TimeSpan.TryParse(entry["StartTime"]?.ToString(), out var st) ? st : TimeSpan.Zero,
                        EndTime = TimeSpan.TryParse(entry["EndTime"]?.ToString(), out var et) ? et : TimeSpan.Zero,
                        Description = entry["Description"]?.ToString() ?? string.Empty
                    };

                    EventsForSelectedDate.Add(ev);
                }
            }
            finally
            {
                _isLoadingEvents = false;
            }
        }



        private async void OnAddEvent()
        {
            // העבר את SelectedDate כפרמטר
            await Shell.Current.GoToAsync(nameof(AddEventPage), new Dictionary<string, object>
            {
                { "EventDate", SelectedDate }
            });
        }

        private async void OnViewEvent(Event? ev)
        {
            if (ev == null) return;
            await Shell.Current.GoToAsync(nameof(EventDetailPage), new Dictionary<string, object>
            {
                ["EventId"] = ev.Id
            });
        }
    }
}
