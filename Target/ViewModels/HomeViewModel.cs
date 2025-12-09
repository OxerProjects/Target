using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly UserService uv;
        private string? title;

        public string? Title
        {
            get => title;
            set { title = value; OnPropertyChanged(nameof(Title)); }
        }

        public HomeViewModel()
        {
            uv = new UserService();
            InitializeTitleAsync();
        }

        private async void InitializeTitleAsync()
        {
            var email = await SecureStorage.GetAsync("userEmail");
            string fullName;

            if (!string.IsNullOrEmpty(email))
            {
                fullName = await uv.GetUserFullNameByEmailAsync(email) ?? string.Empty;
                if (!string.IsNullOrEmpty(fullName))
                    Preferences.Default.Set("userFullName", fullName);
            }

            fullName = "אין שם מוזן";

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
    }
}
