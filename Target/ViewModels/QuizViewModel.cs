using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Target.Models;
using Target.Services;

namespace Target.ViewModels
{
    public partial class QuizViewModel : ObservableObject
    {
        private string question;
        public string? Question
        {
            get => question;
            set
            {
                question = value;
                OnPropertyChanged();
            }
        }
        private int correctIndex;

        public string[] Answers { get; } = new string[4];

        public IRelayCommand<int> AnswerCommand { get; }
        public string Result { get; private set; }

        public QuizViewModel()
        {
            AnswerCommand = new RelayCommand<int>(CheckAnswer);
            LoadQuestionAsync();
        }

        private async void LoadQuestionAsync()
        {
            AiService aiService1 = new AiService(new HttpClient(), "AIzaSyCZy2DIL93VsmgEZzJ3tnfscuT5Q71v6O8");

            question = await aiService1.GenerateAsync("Create an Question about the Israeli militery and units, in Hebrew, the Answers are spoze to be short, write only the Question without other text.");

            Question = question;

            Random rand = new Random();
            int correct = rand.Next(4);
            correctIndex = correct;
            for (int i = 0; i < 4; i++)
            {
                if(i == correct)
                    Answers[i] = await aiService1.GenerateAsync($"Create a Correct Answer of {question}, in Hebrew, make it short max 5 words and optemized, write only the answer without other text.");
                else
                    Answers[i] = await aiService1.GenerateAsync($"Create an uncorrect Answer of {question}, in Hebrew, make it short max 5 words and optemized, write only the answer without other text.");

            }
            OnPropertyChanged(nameof(Answers));
        }

        private void CheckAnswer(int index)
        {
            Result = index == correctIndex
                ? "✅ תשובה נכונה!"
                : "❌ תשובה לא נכונה";
        }
    }
}
