using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public class AddEventViewModel : ObservableObject
    {
        private readonly FirebaseService firebaseService;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "אחר";
        public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;

        public DateTime EventDate { get; set; } = DateTime.Today;

        public IRelayCommand SaveEventCommand { get; }

        public AddEventViewModel(FirebaseService service)
        {
            firebaseService = service;
            SaveEventCommand = new RelayCommand(SaveEvent);
        }

        private async void SaveEvent()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Shell.Current.DisplayAlert("שגיאה", "אנא הזן כותרת לאירוע.", "אישור");
                return;
            }

            var ev = new Event
            {
                Id = Guid.NewGuid().ToString(),
                Date = EventDate,
                Title = Title,
                Description = Description,
                Type = Type,
                StartTime = StartTime,
                EndTime = EndTime
            };

            // ------------- Save to Firebase -------------
            await firebaseService.SaveDocumentAsync("events", ev.Id, new Dictionary<string, object>
            {
                ["Id"] = ev.Id,
                ["Date"] = ev.Date.ToString("yyyy-MM-dd"),
                ["Title"] = ev.Title,
                ["Description"] = ev.Description,
                ["Type"] = ev.Type,
                ["StartTime"] = ev.StartTime.ToString(),
                ["EndTime"] = ev.EndTime.ToString()
            });

            // ---------------- Back -----------------
            await Shell.Current.GoToAsync("..");
        }
    }
}
