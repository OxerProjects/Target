using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;
using System.Collections.ObjectModel;

namespace Target.ViewModels
{
    [QueryProperty(nameof(EventData), "WorkoutEvent")]
    public partial class WorkoutDetailViewModel : ObservableObject
    {
        private readonly FirebaseService _firebaseService;

        [ObservableProperty]
        Event eventData;

        [ObservableProperty]
        string pageTitle;

        public ObservableCollection<WorkoutsDataService.ExerciseDetail> Exercises { get; } = new();

        public WorkoutDetailViewModel(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        partial void OnEventDataChanged(Event value)
        {
            if (value != null)
            {
                PageTitle = value.Title;
                // טעינת התרגילים על בסיס RelatedUnit ששמור ב-Event
                LoadExercises(value.RelatedUnit);
            }
        }

        private void LoadExercises(string unitName)
        {
            Exercises.Clear();
            if (string.IsNullOrEmpty(unitName)) return;

            var workoutData = WorkoutsDataService.GetWorkoutByUnit(unitName);

            if (workoutData != null && workoutData.Exercises != null)
            {
                foreach (var ex in workoutData.Exercises)
                {
                    Exercises.Add(ex);
                }
            }
        }

        [RelayCommand]
        async Task DeleteSingleWorkout()
        {
            bool answer = await Shell.Current.DisplayAlert("מחיקת אימון", "האם למחוק את האימון הספציפי הזה?", "מחק", "ביטול");
            if (answer)
            {
                await _firebaseService.DeleteDocumentAsync("events", EventData.Id);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        async Task DeleteEntirePlan()
        {
            if (string.IsNullOrEmpty(EventData.PlanGroupId))
            {
                await Shell.Current.DisplayAlert("שגיאה", "לא ניתן למחוק סדרה עבור אירוע זה", "אוקיי");
                return;
            }

            bool answer = await Shell.Current.DisplayAlert("מחיקת תוכנית", "פעולה זו תמחק את כל האימונים בסדרה זו. האם להמשיך?", "מחק הכל", "ביטול");

            if (answer)
            {
                var allEvents = await _firebaseService.GetAllDocumentsAsync("events");
                if (allEvents != null)
                {
                    var eventsToDelete = allEvents
                        .Where(e => e.Value.ContainsKey("PlanGroupId") && e.Value["PlanGroupId"]?.ToString() == EventData.PlanGroupId)
                        .Select(e => e.Key)
                        .ToList();

                    foreach (var id in eventsToDelete)
                    {
                        await _firebaseService.DeleteDocumentAsync("events", id);
                    }

                    await Shell.Current.DisplayAlert("הצלחה", "התוכנית הוסרה.", "אוקיי");
                    await Shell.Current.GoToAsync("..");
                }
            }
        }
    }
}