namespace UniversityAPI.Models
{
    public class TeacherProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public List<TeacherGroupSubject> TeacherGroupSubjects { get; set; } = [];
        public List<Group> Groups { get; set; } = [];
        public List<Subject> Subjects { get; set; } = [];
        public int DegreeId { get; set; }
        public required Degree Degree { get; set; }
        public int FacultyId { get; set; }
        public required Faculty Faculty { get; set; }
    }
}