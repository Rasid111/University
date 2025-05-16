
namespace UniversityAPI.Dtos
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CorrectAnswerTitle { get; set; }
        public int TestId { get; set; }
        public List<string> AnswerTitles { get; set; }
    }

    public class CreateQuestionDto
    {
        public string Title { get; set; }
        public string CorrectAnswerTitle { get; set; }
        public int TestId { get; set; }
    }

    public class UpdateQuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CorrectAnswerTitle { get; set; }
        public int TestId { get; set; }
    }
}
