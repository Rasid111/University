namespace UniversityAPI.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int CorrectAnswers { get; set; }
        public int Mistakes { get; set; }  

        public Test Test { get; set; }  
    }
}
