namespace Target.Views;
using Target.ViewModels;

public partial class WorkoutDetailPage : ContentPage
{
	public WorkoutDetailPage()
	{
		InitializeComponent();
		BindingContext = new WorkoutDetailViewModel(App.FirebaseService);
    }
}