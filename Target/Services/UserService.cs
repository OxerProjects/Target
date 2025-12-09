using Target.Helpers;

namespace Target.Services
{
    public class UserService : FirebaseService
    {
        // Register:
        public async Task RegisterUserAsync(Target.Models.User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required", nameof(user.Email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required", nameof(password));

            // Register the user in Firebase Authentication
            var authUser = await RegisterAsync(user.Email!, password);

            if (authUser == null || authUser.User == null)
                throw new Exception("Firebase registration failed.");

            var firebaseId = authUser.User.Uid;

            // Prepare dictionary safely
            var userData = new Dictionary<string, object>
            {
                { Constants.FullName, user.FullName ?? string.Empty },
                { Constants.Email, user.Email ?? string.Empty },
                { Constants.MobileNo, user.MobileNo ?? string.Empty },
                { Constants.BirthDate, user.BirthDate.ToString("yyyy-MM-dd") }
            };

            await SaveDocumentAsync(Constants.UsersCollection, firebaseId, userData);
        }

        // Logs in an existing user using Firebase Authentication
        public async Task<Models.User?> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var authUser = await LoginAsync(email, password);
            if (authUser == null || authUser.User == null)
                return null;

            var firebaseId = authUser.User.Uid;

            var userData = await GetDocumentAsync(Constants.UsersCollection, firebaseId);

            if (userData == null)
                return null;

            return new Models.User
            {
                FullName = userData.TryGetValue(Constants.FullName, out var fullName) ? fullName?.ToString() : string.Empty,
                Email = userData.TryGetValue(Constants.Email, out var emailVal) ? emailVal?.ToString() : string.Empty,
                MobileNo = userData.TryGetValue(Constants.MobileNo, out var mobile) ? mobile?.ToString() : string.Empty,
                BirthDate = userData.TryGetValue(Constants.BirthDate, out var birth) &&
                            DateTime.TryParse(birth?.ToString(), out var parsed)
                                ? parsed
                                : DateTime.MinValue
            };
        }

        public Task<string> GetUserIdAsync()
        {
            return Task.FromResult(auth?.User?.Uid ?? string.Empty);
        }

        public async Task<string?> GetUserFullNameByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            try
            {
                var users = await GetAllDocumentsAsync(Constants.UsersCollection);

                if (users == null)
                    return null;

                foreach (var user in users.Values)
                {
                    if (user.TryGetValue(Constants.Email, out var userEmail))
                    {
                        var emailStr = userEmail?.ToString();

                        if (string.Equals(emailStr, email, StringComparison.OrdinalIgnoreCase))
                        {
                            if (user.TryGetValue(Constants.FullName, out var fullName))
                                return fullName?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching full name by email: {ex.Message}");
            }

            return null;
        }
    }
}
