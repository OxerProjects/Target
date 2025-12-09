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

        private string? fullName;
        private string? email;
        private string? mobileNo;
        private string? password;
        private DateTime birthDate = DateTime.Now;
        private string weight;
        private string height;

        // Validation error messages
        private string? fullNameError;
        private string? emailError;
        private string? mobileNoError;
        private string? passwordError;
        private string? birthDateError;
        private string? weightError;
        private string? heightError;
        #endregion

        #region Commands
        public ICommand RegisterCommand { get; private set; }
        public ICommand NavigateToSignInCommand { get; }
        #endregion

        #region Constructor
        public RegisterViewModel(UserService userService)
        {
            fullName = string.Empty;
            email = string.Empty;
            mobileNo = string.Empty;
            password = string.Empty;
            birthDate = DateTime.Now;
            weight = string.Empty;
            height = string.Empty;

            this.usrService = userService;

            RegisterCommand = new Command(OnRegister);
            NavigateToSignInCommand = new Command(OnNavigateToSignIn);
        }
        #endregion

        #region Properties
        public string? FullName
        {
            get => fullName;
            set
            {
                if (fullName != value)
                {
                    fullName = value;
                    OnPropertyChanged(nameof(FullName));
                    HandleError(nameof(FullName));
                }
            }
        }

        public string? Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                    HandleError(nameof(Email));
                }
            }
        }

        public string? MobileNo
        {
            get => mobileNo;
            set
            {
                if (mobileNo != value)
                {
                    mobileNo = value;
                    OnPropertyChanged(nameof(MobileNo));
                    HandleError(nameof(MobileNo));
                }
            }
        }

        public string? Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                    HandleError(nameof(Password));
                }
            }
        }

        public DateTime BirthDate
        {
            get => birthDate;
            set
            {
                if (birthDate != value)
                {
                    birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                    OnPropertyChanged(nameof(Age));
                    HandleError(nameof(BirthDate));
                }
            }
        }

        public string Weight
        {
            get => weight;
            set
            {
                if (weight != value)
                {
                    weight = value;
                    OnPropertyChanged(nameof(Weight));
                    HandleError(nameof(Weight));
                }
            }
        }

        public string Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged(nameof(Height));
                    HandleError(nameof(Height));
                }
            }
        }

        public int Age => (DateTime.Now.Year - BirthDate.Year) -
            (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

        // Error messages
        public string? FullNameError { get => fullNameError; set { fullNameError = value; OnPropertyChanged(nameof(FullNameError)); } }
        public string? EmailError { get => emailError; set { emailError = value; OnPropertyChanged(nameof(EmailError)); } }
        public string? MobileNoError { get => mobileNoError; set { mobileNoError = value; OnPropertyChanged(nameof(MobileNoError)); } }
        public string? PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(nameof(PasswordError)); } }
        public string? BirthDateError { get => birthDateError; set { birthDateError = value; OnPropertyChanged(nameof(BirthDateError)); } }
        public string? WeightError { get => weightError; set { weightError = value; OnPropertyChanged(nameof(WeightError)); } }
        public string? HeightError { get => heightError; set { heightError = value; OnPropertyChanged(nameof(HeightError)); } }

        public bool HasError =>
            !string.IsNullOrEmpty(FullNameError) ||
            !string.IsNullOrEmpty(PasswordError) ||
            !string.IsNullOrEmpty(EmailError) ||
            !string.IsNullOrEmpty(BirthDateError) ||
            !string.IsNullOrEmpty(MobileNoError) ||
            !string.IsNullOrEmpty(WeightError) ||
            !string.IsNullOrEmpty(HeightError);

        public bool CanRegister => !HasError;
        #endregion

        #region Methods
        private void HandleError(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(FullName):
                    FullNameError = ValidateFullName() ? string.Empty : "שם מלא לא יכול לכלול מספרים או סימנים נוספים.";
                    break;
                case nameof(Email):
                    EmailError = ValidateEmail() ? string.Empty : "דרוש אמייל תקין";
                    break;
                case nameof(MobileNo):
                    MobileNoError = ValidateMobileNumber() ? string.Empty : "מספר טלפון חייב להיות באורך 10 ספרות.";
                    break;
                case nameof(Password):
                    PasswordError = ValidatePassword() ? string.Empty : "הסיסמה חייבת לכלות אות גדולה ומספרים.";
                    break;
                case nameof(BirthDate):
                    BirthDateError = ValidateBirthDate() ? string.Empty : "אתה חייב להיות לפחות בן 12.";
                    break;
                case nameof(Weight):
                    WeightError = ValidateWeight() ? string.Empty : "משקל חייב להיות משקל תקני.";
                    break;
                case nameof(Height):
                    HeightError = ValidateHeight() ? string.Empty : "גובה חייב להיות תקני.";
                    break;
            }

            OnPropertyChanged(nameof(CanRegister));
            OnPropertyChanged(nameof(HasError));
        }

        private bool ValidateFullName() =>
          !string.IsNullOrEmpty(FullName) &&
          Regex.IsMatch(FullName, @"^[a-zA-Z\u0590-\u05FF\s']+$");

        private bool ValidateEmail()
        {
            if (string.IsNullOrEmpty(Email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateMobileNumber() =>
            !string.IsNullOrEmpty(MobileNo) &&
            Regex.IsMatch(MobileNo, @"^\d{10}$");

        private bool ValidatePassword() =>
            !string.IsNullOrEmpty(Password) &&
            Regex.IsMatch(Password, @"^(?=.*[A-Z])(?=.*\d).+$");

        private bool ValidateBirthDate()
        {
            var age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.DayOfYear < BirthDate.DayOfYear) age--;
            return age >= 12;
        }

        private bool ValidateWeight()
        {
            // מוודא שהמשקל הוא מספר בין 40 ל-200
            return double.TryParse(Weight, out double w) && w >= 40 && w <= 200;
        }

        private bool ValidateHeight()
        {
            // מוודא שהגובה הוא מספר בין 140 ל-210
            return double.TryParse(Height, out double h) && h >= 140 && h <= 210;
        }


        private async void OnRegister()
        {
            if (!HasError)
            {
                try
                {
                    User user = CreateUser();
                    if (user.Password != null)
                        await usrService.RegisterUserAsync(user, user.Password);
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("הצלחת", "נרשמת בהצלחה למערכת", "אישור");

                    if (Shell.Current != null)
                        await Shell.Current.GoToAsync("//LogIn");
                }
                catch (Exception ex)
                {
                    if (Application.Current?.MainPage != null)
                        await Application.Current.MainPage.DisplayAlert("שגיאה", $"הרשמה נחשלה: {ex.Message}", "אישור");
                }
            }
            else
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("שגיאה", "אנא תקן את הבעיות ונסה שוב.", "אישור");
            }
        }

        private User CreateUser()
        {
            return new User
            {
                FullName = FullName,
                Email = Email,
                MobileNo = MobileNo,
                Password = Password,
                BirthDate = BirthDate,
                Weight = Weight,
                Height = Height
            };
        }

        private async void OnNavigateToSignIn()
        {
            try
            {
                await Shell.Current.GoToAsync("//LogIn");
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("ניווט נכשל", ex.Message, "אישור");
            }
        }
        #endregion
    }
}
