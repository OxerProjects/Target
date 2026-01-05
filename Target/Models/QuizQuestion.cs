namespace Target.Models
{
    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string[] Answers { get; set; } = new string[4];
        public int CorrectIndex { get; set; }
    }
}
