
namespace UniversityAPI.Dtos
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public List<string> QuestionTitles { get; set; }
    }

    public class CreateTestDto
    {
        public string Title { get; set; }
        public int SubjectId { get; set; }
    }

    public class UpdateTestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int SubjectId { get; set; }
    }
}
