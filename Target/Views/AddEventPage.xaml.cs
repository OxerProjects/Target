using Target.Services;
using Target.ViewModels;

namespace Target.Views;

public partial class AddEventPage : ContentPage
{
    private readonly AddEventViewModel _vm;

    // בנאי ריק — חובה ל־Shell
    public AddEventPage()
    {
        InitializeComponent();
        _vm = new AddEventViewModel(new FirebaseService());
        BindingContext = _vm;
    }

    // אם תרצה בנאי מיוחד – תשאיר אותו גם
    public AddEventPage(FirebaseService service, DateTime eventDate)
    {
        InitializeComponent();
        _vm = new AddEventViewModel(service)
        {
            EventDate = eventDate
        };
        BindingContext = _vm;
    }
}
