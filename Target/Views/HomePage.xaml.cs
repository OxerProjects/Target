using Target.ViewModels;

namespace Target.Views;

public partial class HomePage : ContentPage
{
    // מזריק את ה-ViewModel באופן אוטומטי (Dependency Injection)
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // פונקציה זו רצה בכל פעם שהדף עולה למסך (גם כשחוזרים אליו מדף אחר)
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // רענון הנתונים - חשוב מאוד כדי לראות שינויים אם מחקת אימון בדף הפרטים
        if (BindingContext is HomeViewModel vm)
        {
            await vm.RefreshDataAsync();
        }
    }
}