namespace UniversityBlazor.ViewModels
{
    public class TeacherProfile
    {
        public int Id { get; set; }

        public required string UserId { get; set; }
        public required User User { get; set; }
        public List<TeacherGroupSubject> TeacherGroupSubjects { get; set; } = [];
        public int DegreeId { get; set; }
        public required Degree Degree { get; set; }
        public int FacultyId { get; set; }
        public required Faculty Faculty { get; set; }
    }
}