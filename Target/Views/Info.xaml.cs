using Target.ViewModels;

namespace Target.Views;

public partial class Info : ContentPage
{
	public Info()
	{
		InitializeComponent();
        BindingContext = new InfoViewModel();
    }
}