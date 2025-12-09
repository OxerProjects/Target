using System.Collections.ObjectModel;
using System.Windows.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public class InfoViewModel : BindableObject
    {
        private readonly FirebaseService _firebaseService;

        public ObservableCollection<Unit> Units { get; set; } = new();

        public ICommand UnitTappedCommand { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public InfoViewModel()
        {
            _firebaseService = new FirebaseService();
            UnitTappedCommand = new Command<Unit>(OnUnitTapped);
            _ = LoadUnitsAsync();
        }

        private async Task LoadUnitsAsync()
        {
            try
            {
                IsLoading = true;

                var unitsFromDb = await _firebaseService.GetAllUnitsAsync();

                Units.Clear();
                foreach (var unit in unitsFromDb)
                {
                    // עדכון הנתיב המלא ללוגו ולתמונה
                    if (!string.IsNullOrEmpty(unit.Logo))
                        unit.Logo = $"{unit.Logo}";

                    if (!string.IsNullOrEmpty(unit.UnitImage))
                        unit.UnitImage = $"{unit.UnitImage}";

                    Units.Add(unit);
                }
            }
            catch (System.Exception ex)
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert("שגיאה", $"טעינת היחידות נכשלה: {ex.Message}", "אוקיי");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnUnitTapped(Unit unit)
        {
            if (unit == null) return;

            // השתמשנו בשדה Name בעברית, Id לא מתאים לניווט UI
            await Shell.Current.GoToAsync($"{nameof(Target.Views.InfoDetailPage)}?UnitName={unit.Title}");
        }
    }
}
