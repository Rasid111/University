namespace UniversityAPI.Models
{
    public class Test
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        public List<Question> Questions { get; set; } = [];
        public int SubjectId { get; set; }
        public required Subject Subject { get; set; }
    }
}
