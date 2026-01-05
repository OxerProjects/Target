using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
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

        public IRelayCommand<Event> ViewEventCommand { get; }

        #endregion

        #region Quiz

        public IRelayCommand GoToQuizCommand { get; }

        #endregion

        public HomeViewModel()
        {
            uv = new UserService();
            firebaseService = new FirebaseService();

            ViewEventCommand = new RelayCommand<Event>(ViewEvent);
            GoToQuizCommand = new RelayCommand(GoToQuiz);

            InitializeTitleAsync();
            //LoadTodayEventsAsync();
        }

        #region Greeting Title

        private async void InitializeTitleAsync()
        {
            string fullName = "אין שם מוזן";

            var email = await SecureStorage.GetAsync("userEmail");
            if (!string.IsNullOrEmpty(email))
            {
                var nameFromDb = await uv.GetUserFullNameByEmailAsync(email);
                if (!string.IsNullOrEmpty(nameFromDb))
                {
                    fullName = nameFromDb;
                    Preferences.Default.Set("userFullName", fullName);
                }
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

        #endregion

        #region Load Today Events

        private async void LoadTodayEventsAsync()
        {
            TodayEvents.Clear();

            string userEmail = Preferences.Default.Get("userEmail", string.Empty);
            if (string.IsNullOrEmpty(userEmail))
            {
                HasTodayEvents = false;
                return;
            }

            var allEvents = await firebaseService.GetAllDocumentsAsync("events");
            if (allEvents == null)
            {
                HasTodayEvents = false;
                return;
            }

            DateTime today = DateTime.Today;

            foreach (var entry in allEvents.Values)
            {
                if (!entry.TryGetValue("CreatorEmail", out var creatorObj)) continue;
                if (!creatorObj?.ToString()
                    .Equals(userEmail, StringComparison.OrdinalIgnoreCase) == true)
                    continue;

                if (!entry.TryGetValue("Date", out var dateObj)) continue;
                if (!DateTime.TryParse(dateObj?.ToString(), out var eventDate)) continue;
                if (eventDate.Date != today) continue;

                var ev = new Event
                {
                    Id = entry.ContainsKey("Id")
                        ? entry["Id"]?.ToString() ?? Guid.NewGuid().ToString()
                        : Guid.NewGuid().ToString(),

                    Title = entry["Title"]?.ToString() ?? string.Empty,
                    Description = entry["Description"]?.ToString() ?? string.Empty,
                    CreatorEmail = userEmail,
                    Date = eventDate,
                    StartTime = TimeSpan.TryParse(entry["StartTime"]?.ToString(), out var st)
                        ? st
                        : TimeSpan.Zero,
                    EndTime = TimeSpan.TryParse(entry["EndTime"]?.ToString(), out var et)
                        ? et
                        : TimeSpan.Zero,
                    Type = entry["Type"]?.ToString() ?? "אחר"
                };

                TodayEvents.Add(ev);

                // מקסימום 2 אירועים
                if (TodayEvents.Count == 2)
                    break;
            }

            HasTodayEvents = TodayEvents.Any();
        }

        public async Task RefreshTodayEventsAsync()
        {
            TodayEvents.Clear();

            string userEmail = Preferences.Default.Get("userEmail", string.Empty);
            if (string.IsNullOrEmpty(userEmail))
            {
                HasTodayEvents = false;
                return;
            }

            var allEvents = await firebaseService.GetAllDocumentsAsync("events");
            if (allEvents == null)
            {
                HasTodayEvents = false;
                return;
            }

            DateTime today = DateTime.Today;

            foreach (var entry in allEvents.Values)
            {
                if (!entry.TryGetValue("CreatorEmail", out var creatorObj)) continue;
                if (!creatorObj?.ToString()
                    .Equals(userEmail, StringComparison.OrdinalIgnoreCase) == true)
                    continue;

                if (!entry.TryGetValue("Date", out var dateObj)) continue;
                if (!DateTime.TryParse(dateObj?.ToString(), out var eventDate)) continue;
                if (eventDate.Date != today) continue;

                TodayEvents.Add(new Event
                {
                    Id = entry["Id"]?.ToString() ?? Guid.NewGuid().ToString(),
                    Title = entry["Title"]?.ToString() ?? string.Empty,
                    Description = entry["Description"]?.ToString() ?? string.Empty,
                    CreatorEmail = userEmail,
                    Date = eventDate,
                    StartTime = TimeSpan.TryParse(entry["StartTime"]?.ToString(), out var st) ? st : TimeSpan.Zero,
                    EndTime = TimeSpan.TryParse(entry["EndTime"]?.ToString(), out var et) ? et : TimeSpan.Zero,
                    Type = entry["Type"]?.ToString() ?? "אחר"
                });

                if (TodayEvents.Count == 2)
                    break;
            }

            HasTodayEvents = TodayEvents.Any();
        }


        #endregion

        #region Navigation

        private async void ViewEvent(Event ev)
        {
            if (ev == null)
                return;

            await Shell.Current.GoToAsync(
                nameof(Target.Views.EventDetailPage),
                new Dictionary<string, object>
                {
                    ["EventId"] = ev.Id
                });
        }

        private async void GoToQuiz()
        {
            await Shell.Current.GoToAsync(nameof(Target.Views.Quiz));
        }

        #endregion
    }
}
