using Target.ViewModels;

namespace Target.Views;

public partial class EventDetailPage : ContentPage
{
	public EventDetailPage(EventDetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}