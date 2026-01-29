using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using Target.Services;

namespace Target.ViewModels
{
    public partial class QuizViewModel : ObservableObject
    {
        private readonly AiService _aiService;

        // --- Properties ---

        private string question;
        public string Question
        {
            get => question;
            set { question = value; OnPropertyChanged(); }
        }

        private string[] answers = new string[4];
        public string[] Answers
        {
            get => answers;
            set { answers = value; OnPropertyChanged(); }
        }

        private string result;
        public string Result
        {
            get => result;
            set { result = value; OnPropertyChanged(); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged();
                // ברגע ש-IsBusy משתנה, אנחנו מודיעים שגם IsNotBusy השתנה
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        // זה הטריק שחוסך לנו את ה-Converter בשגיאה שקיבלת!
        public bool IsNotBusy => !IsBusy;

        private int correctIndex;
        public IRelayCommand<int> AnswerCommand { get; }
        public IRelayCommand NextQuestionCommand { get; }

        // --- Constructor ---

        public QuizViewModel()
        {
            // שים כאן את המפתח שלך
            _aiService = new AiService(new HttpClient(), "AIzaSyCwLzoBGQBUI1KfEeajtBO_D80g6-EpVfQ");

            AnswerCommand = new RelayCommand<int>(CheckAnswer);
            NextQuestionCommand = new RelayCommand(LoadQuestionAsync);

            LoadQuestionAsync();
        }

        // --- Logic ---

        private async void LoadQuestionAsync()
        {
            if (IsBusy) return;
            IsBusy = true; // נועל כפתורים ומראה טעינה

            Result = "טוען שאלה חדשה...";
            Question = "";
            Answers = new string[] { "", "", "", "" };

            string prompt = @"
                Generate a tough trivia question about IDF (Israel Defense Forces) history, units, or wars in Hebrew.
                Output valid JSON only using this schema:
                {
                  ""q"": ""Question text in Hebrew"",
                  ""c"": ""Correct Answer (max 4 words)"",
                  ""w"": [""Wrong1"", ""Wrong2"", ""Wrong3""]
                }";

            var jsonResponse = await _aiService.GenerateAsync(prompt);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Result = "שגיאה בטעינה. בדוק אינטרנט או מפתח API.";
                IsBusy = false;
                return;
            }

            try
            {
                jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();

                int startIndex = jsonResponse.IndexOf('{');
                int endIndex = jsonResponse.LastIndexOf('}');
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    jsonResponse = jsonResponse.Substring(startIndex, endIndex - startIndex + 1);
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var quizData = JsonSerializer.Deserialize<QuizData>(jsonResponse, options);

                if (quizData != null)
                {
                    Question = quizData.q;
                    Random rand = new Random();
                    correctIndex = rand.Next(4);

                    var tempAnswers = new string[4];
                    int wrongCount = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        if (i == correctIndex)
                            tempAnswers[i] = quizData.c;
                        else
                            tempAnswers[i] = quizData.w[wrongCount++];
                    }

                    Answers = tempAnswers;
                    Result = "";
                }
            }
            catch (Exception ex)
            {
                Result = "שגיאה בפענוח הנתונים.";
            }
            finally
            {
                IsBusy = false; // משחרר את הכפתורים
            }
        }

        private void CheckAnswer(int index)
        {
            if (string.IsNullOrEmpty(Answers[0])) return;

            if (index == correctIndex)
                Result = "✅ נכון מאוד! (לחץ לטעון שאלה הבאה)";
            else
                Result = "❌ טעות, נסה שוב.";
        }
    }

    public class QuizData
    {
        public string q { get; set; }
        public string c { get; set; }
        public List<string> w { get; set; }
    }
}