using Target.ViewModels;

namespace Target.Views;
public partial class Quiz : ContentPage
{
    public Quiz(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}