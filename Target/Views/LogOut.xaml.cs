namespace Target.Views;

public partial class LogOut : ContentPage
{
	public LogOut()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            // Clear user data from secure storage
            SecureStorage.Remove("userEmail");

            // Go to MainPage after logout
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("שגיאה", $"יציאה נכשלה: {ex.Message}", "אישור");
        }
    }
}