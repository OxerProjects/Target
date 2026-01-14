using Target.ViewModels;

namespace Target.Views;

public partial class AddTrainingPlanPage : ContentPage
{
    // מזריקים את ה-ViewModel בבנאי
    public AddTrainingPlanPage(AddTrainingPlanViewModel viewModel)
    {
        InitializeComponent();
        // מחברים את הדף ל-ViewModel
        BindingContext = viewModel;
    }
}