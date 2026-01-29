using System.Text.RegularExpressions;
using System.Windows.Input;
using Target.Services;
using Target.Models;

namespace Target.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        #region Fields
        private readonly UserService usrService;

        private string? fullName = string.Empty;
        private string? email = string.Empty;
        private string? mobileNo = string.Empty;
        private string? password = string.Empty;
        private DateTime birthDate = DateTime.Now;
        private string weight = string.Empty;
        private string height = string.Empty;

        // שדה שגיאה מאוחד
        private string? errorMessage;
        #endregion

        #region Commands
        public ICommand RegisterCommand { get; private set; }
        public ICommand NavigateToSignInCommand { get; }
        #endregion

        #region Constructor
        public RegisterViewModel(UserService userService)
        {
            this.usrService = userService;
            RegisterCommand = new Command(OnRegister);
            NavigateToSignInCommand = new Command(OnNavigateToSignIn);
        }
        #endregion

        #region Properties
        public string? FullName { get => fullName; set { if (SetProperty(ref fullName, value)) ValidateForm(); } }
        public string? Email { get => email; set { if (SetProperty(ref email, value)) ValidateForm(); } }
        public string? MobileNo { get => mobileNo; set { if (SetProperty(ref mobileNo, value)) ValidateForm(); } }
        public string? Password { get => password; set { if (SetProperty(ref password, value)) ValidateForm(); } }
        public DateTime BirthDate
        {
            get => birthDate;
            set
            {
                if (birthDate != value)
                {
                    birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                    OnPropertyChanged(nameof(Age)); // מודיע ל-UI שהגיל השתנה
                    ValidateForm(); // בודק אם הגיל החדש תקין (מעל 12)
                }
            }
        }
        public string Weight { get => weight; set { if (SetProperty(ref weight, value)) ValidateForm(); } }
        public string Height { get => height; set { if (SetProperty(ref height, value)) ValidateForm(); } }
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string? ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public bool CanRegister => string.IsNullOrEmpty(ErrorMessage);
        #endregion

        #region Methods
        private void ValidateForm()
        {
            if (!ValidateFullName())
                ErrorMessage = "שם מלא לא יכול לכלול מספרים או סימנים נוספים.";
            else if (!ValidateEmail())
                ErrorMessage = "דרוש אימייל תקין.";
            else if (!ValidateMobileNumber())
                ErrorMessage = "מספר טלפון חייב להיות באורך 10 ספרות.";
            else if (!ValidatePassword())
                ErrorMessage = "הסיסמה חייבת לכלול אות גדולה ומספרים.";
            else if (!ValidateBirthDate())
                ErrorMessage = "אתה חייב להיות לפחות בן 12.";
            else if (!ValidateWeight())
                ErrorMessage = "משקל חייב להיות בין 40 ל-200 ק\"ג.";
            else if (!ValidateHeight())
                ErrorMessage = "גובה חייב להיות בין 140 ל-210 ס\"מ.";
            else
                ErrorMessage = string.Empty;

            OnPropertyChanged(nameof(CanRegister));
        }

        // --- פונקציות וולידציה ---
        private bool ValidateFullName() => !string.IsNullOrWhiteSpace(FullName) && Regex.IsMatch(FullName, @"^[a-zA-Z\u0590-\u05FF\s']+$");

        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            try { return new System.Net.Mail.MailAddress(Email).Address == Email; }
            catch { return false; }
        }

        private bool ValidateMobileNumber() => !string.IsNullOrEmpty(MobileNo) && Regex.IsMatch(MobileNo, @"^\d{10}$");

        private bool ValidatePassword() => !string.IsNullOrEmpty(Password) && Regex.IsMatch(Password, @"^(?=.*[A-Z])(?=.*\d).+$");

        private bool ValidateBirthDate()
        {
            return Age >= 12; // פשוט משתמש ב-Property שחישבנו
        }
        private bool ValidateWeight() => double.TryParse(Weight, out double w) && w >= 40 && w <= 200;

        private bool ValidateHeight() => double.TryParse(Height, out double h) && h >= 140 && h <= 210;


        private async void OnRegister()
        {
            ValidateForm(); // וידוא אחרון לפני שליחה
            if (CanRegister)
            {
                try
                {
                    User user = CreateUser();
                    await usrService.RegisterUserAsync(user, user.Password!);

                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("הצלחת", "נרשמת בהצלחה למערכת", "אישור");

                    await Shell.Current.GoToAsync("//LogIn");
                }
                catch (Exception ex)
                {
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("שגיאה", $"הרשמה נכשלה: {ex.Message}", "אישור");
                }
            }
        }

        private User CreateUser() => new User
        {
            FullName = FullName,
            Email = Email,
            MobileNo = MobileNo,
            Password = Password,
            BirthDate = BirthDate,
            Weight = Weight,
            Height = Height
        };

        private async void OnNavigateToSignIn()
        {
            try { await Shell.Current.GoToAsync("//LogIn"); }
            catch (Exception ex) { /* טיפול בשגיאה */ }
        }
        #endregion
    }
}