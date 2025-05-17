namespace UniversityBlazor.ViewModels
{
    public class StudentProfile
    {
        public int Id { get; set; }

        public required string UserId { get; set; }
        public required User User { get; set; }
        public int GroupId { get; set; }
        public required Group Group { get; set; }
        public List<Grade> Grades { get; set; } = [];
        public List<TestResult> TestResults { get; set; } = [];
    }
}