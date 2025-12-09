namespace Target.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLogInClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LogIn");
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Register");
    }
}