using Target.ViewModels;

namespace Target.Views;

public partial class Register : ContentPage
{
    public Register(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    private async void GoBack(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}