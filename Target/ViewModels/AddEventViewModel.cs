using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    [QueryProperty(nameof(EventDate), "EventDate")]
    public class AddEventViewModel : ObservableObject
    {
        private readonly FirebaseService firebaseService;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "אחר";
        public string userEmail;
        public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        private DateTime? _eventDate;
        public DateTime EventDate
        {
            get => _eventDate ?? DateTime.Today;
            set
            {
                _eventDate = value;
                OnPropertyChanged();
            }
        }

        public IRelayCommand SaveEventCommand { get; }
        public AddEventViewModel(FirebaseService service)
        {
            firebaseService = service;
            SaveEventCommand = new RelayCommand(SaveEvent);
            userEmail = Preferences.Default.Get("userEmail", string.Empty);
        }

        private async void SaveEvent()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Shell.Current.DisplayAlert("שגיאה", "אנא הזן כותרת לאירוע.", "אישור");
                return;
            }

            userEmail = Preferences.Default.Get("userEmail", string.Empty);

            var ev = new Event
            {
                Id = Guid.NewGuid().ToString(),
                Date = EventDate,
                Title = Title,
                Description = Description,
                Type = Type,
                StartTime = StartTime,
                EndTime = EndTime,
                CreatorEmail = userEmail,       
                Participants = new List<string> { userEmail }
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
                ["EndTime"] = ev.EndTime.ToString(),
                ["CreatorEmail"] = ev.CreatorEmail,
                ["Participants"] = ev.Participants // this will save as a list in Firebase
            });


            // ---------------- Back -----------------
            await Shell.Current.GoToAsync("..");
        }
    }
}
