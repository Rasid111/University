namespace UniversityAPI.Models
{
    public class TestAnswer
    {
        public int Id { get; set; }
        public required string Answer { get; set; }
        public required string CorrectAnswer { get; set; }

        public int TestId { get; set; }
        public required Test Test { get; set; }
    }
}
