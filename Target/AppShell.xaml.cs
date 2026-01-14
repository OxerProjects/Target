using Target.Views;

namespace Target
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // --------------------- REGISTER ROUTES ---------------------
            Routing.RegisterRoute(nameof(Target.Views.InfoDetailPage), typeof(Target.Views.InfoDetailPage));
            Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
            Routing.RegisterRoute(nameof(AddTrainingPlanPage), typeof(AddTrainingPlanPage));
            Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
            Routing.RegisterRoute(nameof(AddTrainingPlanPage), typeof(AddTrainingPlanPage));
            Routing.RegisterRoute(nameof(WorkoutDetailPage), typeof(WorkoutDetailPage));
            Routing.RegisterRoute("Calender", typeof(Calendar));
            Routing.RegisterRoute(nameof(Quiz), typeof(Quiz));

            SecureStorage.SetAsync("GeminiKey", "AIzaSyCNeELWH7iYlikfr3qnBEpk7MqVg0IHYSo");


            // --------------------- SET CURRENT DATE ---------------------
            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            Date.Text = Date2.Text = Date3.Text = currentDate;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckUserLogin();
        }

        // --------------------- AUTO LOGIN ---------------------
        private async Task CheckUserLogin()
        {
            try
            {
                var email = await SecureStorage.GetAsync("userEmail");
                if (!string.IsNullOrEmpty(email))
                {
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await Shell.Current.GoToAsync("//LogIn");
                }
            }
            catch (Exception)
            {
                await Shell.Current.GoToAsync("//LogIn");
            }
        }
    }
}
