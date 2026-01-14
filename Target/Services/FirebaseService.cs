using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using Target.Models;

namespace Target.Services
{
    public class FirebaseService
    {
        public readonly FirebaseAuthClient auth;
        protected readonly FirebaseClient firebaseClient;

        public FirebaseService()
        {
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyDJGgupfnMNncMpZTQpiv85bQIhwo4YiBg",
                AuthDomain = "target-database-d8d36.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                },
                UserRepository = new FileUserRepository("FirebaseApp")
            };

            auth = new FirebaseAuthClient(config);
            firebaseClient = new FirebaseClient("https://target-database-d8d36-default-rtdb.firebaseio.com/");
        }

        public async Task<UserCredential> RegisterAsync(string email, string password)
        {
            return await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        public async Task<UserCredential> LoginAsync(string email, string password)
        {
            return await auth.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task SaveDocumentAsync(string collection, string docId, Dictionary<string, object> data)
        {
            try
            {
                await firebaseClient.Child(collection).Child(docId).PutAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
            }
        }

        public async Task UpdateDocumentAsync(string collection, string docId, Dictionary<string, object> data)
        {
            try
            {
                await firebaseClient.Child(collection).Child(docId).PatchAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating document: {ex.Message}");
            }
        }

        public async Task DeleteDocumentAsync(string collection, string docId)
        {
            try
            {
                await firebaseClient.Child(collection).Child(docId).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting document: {ex.Message}");
            }
        }

        //public async Task AddUserWorkoutAsync(UserWorkout workout)
        //{
        //    try
        //    {
        //        await firebaseClient
        //            .Child("UserWorkouts")
        //            .Child(workout.UserId)
        //            .PostAsync(workout);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error saving workout: {ex.Message}");
        //    }
        //}

        public async Task<Dictionary<string, Dictionary<string, object>>?> GetAllDocumentsAsync(string collection)
        {
            try
            {
                var result = await firebaseClient
                    .Child(collection)
                    .OnceAsync<Dictionary<string, object>>();

                return result.ToDictionary(
                    item => item.Key,
                    item => item.Object
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all documents: {ex.Message}");
                return null;
            }
        }


        public async Task<Dictionary<string, object>?> GetDocumentAsync(string collection, string docId)
        {
            try
            {
                var result = await firebaseClient
                    .Child(collection)
                    .Child(docId)
                    .OnceSingleAsync<Dictionary<string, object>>();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching document: {ex.Message}");
                return null;
            }
        }

        public async Task UploadUnitsAsync(List<Unit> units)
        {
            foreach (var unit in units)
            {
                // וודא שיש Id
                if (string.IsNullOrEmpty(unit.Id))
                    unit.Id = Guid.NewGuid().ToString();

                var data = new Dictionary<string, object>
                {
                    { "Id", unit.Id },
                    { "Title", unit.Title ?? string.Empty },
                    { "Description", unit.Description ?? string.Empty },
                    { "Logo", unit.Logo ?? string.Empty },
                    { "UnitImage", unit.UnitImage ?? string.Empty },
                    { "Sector", unit.Sector ?? string.Empty },
                    { "PageName", unit.PageName ?? string.Empty }
                };

                Console.WriteLine($"✅ Uploading {unit.Title}");
                await SaveDocumentAsync("units", unit.Id, data);
            }

            Console.WriteLine("✅ All units uploaded successfully!");
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            try
            {
                // אנחנו שואבים את כל הצומת כרשימה אחת, כי זה פורמט מערך (Array)
                var result = await firebaseClient
                    .Child("units")
                    .OnceSingleAsync<List<Unit>>();

                if (result == null) return new List<Unit>();

                // סינון ה-nullים שהופיעו בלוג שלך ושמירה רק על יחידות אמיתיות
                return result
                    .Where(u => u != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                // זה ידפיס לך בדיוק מה השגיאה ב-Output
                Console.WriteLine($"❌ Error fetching units: {ex.Message}");
                return new List<Unit>();
            }
        }
    }
}
