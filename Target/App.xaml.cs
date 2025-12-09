using Target.Services;
using Target.ViewModels;

namespace Target
{
    public partial class App : Application
    {
        public static FirebaseService? FirebaseService { get; private set; }
        public App(LoginViewModel loginViewModel, AppShell appShell)
        {
            InitializeComponent();

            FirebaseService = new FirebaseService();

            MainPage = appShell;
        }
    }
}
