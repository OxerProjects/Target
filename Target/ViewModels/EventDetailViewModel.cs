using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    [QueryProperty(nameof(EventId), "EventId")]
    public class EventDetailViewModel : ObservableObject
    {
        private readonly FirebaseService firebaseService;

        private string _eventId;
        public string EventId
        {
            get => _eventId;
            set
            {
                Console.WriteLine("===========================================");
                Console.WriteLine(_eventId);
                Console.WriteLine("===========================================");
                _eventId = value;
                LoadEvent(_eventId);
            }
        }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "אחר";
        public DateTime Date { get; set; } = DateTime.Today;
        public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;

        public IRelayCommand SaveEventCommand { get; }
        public IRelayCommand DeleteEventCommand { get; }

        public EventDetailViewModel(FirebaseService service)
        {
            firebaseService = service;
            SaveEventCommand = new RelayCommand(SaveEvent);
            DeleteEventCommand = new RelayCommand(DeleteEvent);
        }

        public async Task LoadEvent(string eventId)
        {
            var doc = await firebaseService.GetDocumentAsync("events", eventId);
            if (doc == null) return;

            Title = doc.TryGetValue("Title", out var t) ? t?.ToString() ?? string.Empty : string.Empty;
            Description = doc.TryGetValue("Description", out var d) ? d?.ToString() ?? string.Empty : string.Empty;
            Type = doc.TryGetValue("Type", out var ty) ? ty?.ToString() ?? "אחר" : "אחר";
            Date = doc.TryGetValue("Date", out var dt) && DateTime.TryParse(dt?.ToString(), out var dateVal)
                ? dateVal
                : DateTime.Today;
            StartTime = doc.TryGetValue("StartTime", out var st) && TimeSpan.TryParse(st?.ToString(), out var stVal)
                ? stVal
                : TimeSpan.Zero;
            EndTime = doc.TryGetValue("EndTime", out var et) && TimeSpan.TryParse(et?.ToString(), out var etVal)
                ? etVal
                : TimeSpan.Zero;

            OnPropertyChanged(string.Empty);
        }


        private async void SaveEvent()
        {
            if (string.IsNullOrWhiteSpace(Title)) return;

            var data = new Dictionary<string, object>
            {
                ["Id"] = EventId,
                ["Title"] = Title,
                ["Description"] = Description,
                ["Type"] = Type,
                ["Date"] = Date.ToString("yyyy-MM-dd"),
                ["StartTime"] = StartTime.ToString(),
                ["EndTime"] = EndTime.ToString()
            };

            await firebaseService.UpdateDocumentAsync("events", EventId, data);
            await Shell.Current.GoToAsync(".."); // חזרה ליומן
        }

        private async void DeleteEvent()
        {
            await firebaseService.DeleteDocumentAsync("events", EventId);
            await Shell.Current.GoToAsync(".."); // חזרה ליומן
        }
    }
}
