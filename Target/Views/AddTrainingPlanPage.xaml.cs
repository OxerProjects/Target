using Target.ViewModels;

namespace Target.Views;

public partial class AddTrainingPlanPage : ContentPage
{
    public AddTrainingPlanPage(AddTrainingPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // שורה זו קריטית!
    }
}