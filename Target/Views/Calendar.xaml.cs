using Target.Services;
using Target.ViewModels;

namespace Target.Views
{
    public partial class Calendar : ContentPage
    {

        public Calendar()
        {
            InitializeComponent();

            var firebaseService = new FirebaseService();
            this.BindingContext = new CalendarViewModel(firebaseService);
        }
    }
}
