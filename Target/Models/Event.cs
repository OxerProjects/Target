namespace Target.Models
{
    public class Event
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Type { get; set; } = "אחר"; // אימון, יום מיון, אחר
        public string Title { get; set; } = string.Empty;
        public string CreatorEmail { get; set; } = string.Empty;
        public List<string> Participants { get; set; } = new();
        public string Description { get; set; } = string.Empty;

        public string? RelatedUnit { get; set; } // שם היחידה (למשל "504")
        public string? PlanGroupId { get; set; } // מזהה ייחודי לכל התוכנית שנוצרה יחד
    }
}
