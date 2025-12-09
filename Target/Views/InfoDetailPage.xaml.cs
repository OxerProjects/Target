using Target.Models;
using Target.Services;

namespace Target.Views;

[QueryProperty(nameof(UnitName), "UnitName")]
public partial class InfoDetailPage : ContentPage
{
    private string? _unitName;
    public string? UnitName
    {
        get => _unitName;
        set
        {
            _unitName = value;
            OnPropertyChanged();
            LoadUnitDetails();
        }
    }
    public Unit? CurrentUnit { get; set; }

    public InfoDetailPage()
	{
		InitializeComponent();
	}

    private async void LoadUnitDetails()
    {
        if (string.IsNullOrEmpty(UnitName))
            return;

        try
        {
            var firebaseService = new FirebaseService();

            // מביא את כל היחידות ואז מוצא את היחידה לפי שם
            var allUnits = await firebaseService.GetAllUnitsAsync();
            if (allUnits != null)
                CurrentUnit = allUnits.FirstOrDefault(u => u.Title == UnitName);

            if (CurrentUnit != null)
            {
                // מחברים ל-BindingContext כדי שה-XAML יציג את הנתונים
                BindingContext = CurrentUnit;
            }
            else
            {
                await DisplayAlert("שגיאה", "לא נמצאה היחידה המבוקשת", "אוקיי");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("שגיאה", $"אירעה שגיאה בטעינת היחידה: {ex.Message}", "אוקיי");
        }
    }
}