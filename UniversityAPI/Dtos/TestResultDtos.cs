
namespace UniversityAPI.Dtos
{
    public class TestResultDto
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public int CorrectAnswers { get; set; }
        public int Mistakes { get; set; }
    }

    public class CreateTestResultDto
    {
        public int TestId { get; set; }
        public int CorrectAnswers { get; set; }
        public int Mistakes { get; set; }
    }

    public class UpdateTestResultDto
    {
        public int Id { get; set; }
        public int CorrectAnswers { get; set; }
        public int Mistakes { get; set; }
    }
}
