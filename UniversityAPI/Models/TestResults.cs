namespace UniversityAPI.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        public List<TestAnswer> Answers { get; set; } = [];
        public int TestId { get; set; }
        public required Test Test { get; set; }  
    }
}
