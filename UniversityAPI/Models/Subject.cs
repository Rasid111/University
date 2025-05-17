namespace UniversityAPI.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public List<TeacherGroupSubject> TeacherGroupSubjects { get; set; } = [];
        public List<Test> Tests { get; set; } = [];
    }
}
