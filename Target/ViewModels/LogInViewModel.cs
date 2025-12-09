using System.Windows.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields
        private readonly UserService userService;
        private string email = string.Empty;
        private string password = string.Empty;
        private bool isPasswordVisible;
        private string emailError = string.Empty;
        private string passwordError = string.Empty;
        private bool isLoading;
        #endregion

        #region Constructor
        public LoginViewModel(UserService userService)
        {
            this.userService = userService;
            isLoading = false;

            NavigateToSignUpCommand = new Command(OnNavigateToSignUp);
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
            LoginCommand = new Command(OnLogin, CanLogin);
            CancelCommand = new Command(OnCancel, CanCancel);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Email) || e.PropertyName == nameof(Password))
                {
                    ((Command)LoginCommand).ChangeCanExecute();
                    ((Command)CancelCommand).ChangeCanExecute();
                    OnPropertyChanged(nameof(IsLoginEnabled));
                }
            };
        }
        #endregion

        #region Properties
        public bool IsLoading
        {
            get => isLoading;
            set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public string Email
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

        public string Password
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

        public bool IsPasswordVisible
        {
            get => isPasswordVisible;
            set
            {
                isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
                OnPropertyChanged(nameof(IsPasswordHidden));
            }
        }

        public bool IsPasswordHidden => !IsPasswordVisible;

        public string EmailError
        {
            get => emailError;
            set { emailError = value; OnPropertyChanged(nameof(EmailError)); }
        }

        public string PasswordError
        {
            get => passwordError;
            set { passwordError = value; OnPropertyChanged(nameof(PasswordError)); }
        }

        public bool IsLoginEnabled => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password) && !HasError;
        public bool IsCancelEnabled => !string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Password);
        public bool HasError => !string.IsNullOrEmpty(EmailError) || !string.IsNullOrEmpty(PasswordError);
        #endregion

        #region Commands
        public ICommand NavigateToSignUpCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Methods
        private void HandleError(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Email):
                    EmailError = string.IsNullOrEmpty(Email) ? "נדרש אימייל" : string.Empty;
                    break;
                case nameof(Password):
                    PasswordError = string.IsNullOrEmpty(Password) ? "נדרשת סיסמה" : string.Empty;
                    break;
            }
        }

        private async void OnNavigateToSignUp()
        {
            try
            {
                await Shell.Current.GoToAsync("//Register");
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("שגיאה בניווט", ex.Message, "אישור");
                }

            }
        }

        private async void OnLogin()
        {
            if (!HasError)
            {
                try
                {
                    IsLoading = true;

                    User? user = await userService.LoginUserAsync(Email, Password);

                    if (user != null)
                    {
                        Preferences.Default.Set("userEmail", Email);
                        Preferences.Default.Set("userFullName", user.FullName);
                        await SecureStorage.SetAsync("userEmail", Email);
                        await SecureStorage.SetAsync("userPassword", Password);

                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        PasswordError = "סיסמה לא תקינה, נסה שוב.";
                        await Shell.Current.DisplayAlert("Login", PasswordError, "אישור");
                    }

                }
                catch (Exception ex)
                {
                    await ShowAlert("שגיאה", $"התחברות נכשלה: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private Task ShowAlert(string title, string message)
        {
            if (Application.Current?.MainPage != null)
                return Application.Current.MainPage.DisplayAlert(title, message, "אישור");
            return Task.CompletedTask;
        }

        private bool CanLogin() => IsLoginEnabled;

        private void OnCancel()
        {
            Email = string.Empty;
            Password = string.Empty;
            ClearErrors();
        }

        private void ClearErrors()
        {
            EmailError = string.Empty;
            PasswordError = string.Empty;
        }

        private bool CanCancel() => IsCancelEnabled;

        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
        #endregion
    }
}
