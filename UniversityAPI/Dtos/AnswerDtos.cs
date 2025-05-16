
namespace UniversityAPI.Dtos
{
    public class AnswerDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int QuestionId { get; set; }
    }

    public class CreateAnswerDto
    {
        public string Title { get; set; }
        public int QuestionId { get; set; }
    }

    public class UpdateAnswerDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int QuestionId { get; set; }
    }
}
