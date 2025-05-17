namespace UniversityBlazor.ViewModels
{
    public class QuestionAnswer
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public int QuestionId { get; set; }
        public required Question Question { get; set; }
    }
}
