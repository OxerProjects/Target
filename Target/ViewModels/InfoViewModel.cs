using System.Collections.ObjectModel;
using System.Diagnostics;
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
                    // בדיקת בטיחות שהיחידה והכותרת קיימים
                    if (unit == null || string.IsNullOrEmpty(unit.Title)) continue;

                    Units.Add(unit);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
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
