using Target.ViewModels;

namespace Target.Views;

public partial class LogIn : ContentPage
{
    public LogIn(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void GoBack(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}