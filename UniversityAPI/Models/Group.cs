namespace UniversityAPI.Models
{
    public class Group
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Year { get; set; }

        public List<TeacherGroupSubject> TeacherGroupSubjects { get; set; } = [];
        public List<StudentProfile> Students { get; set; } = [];
        public int FacultyId { get; set; }
        public required Faculty Faculty { get; set; }
        public int MajorId { get; set; }
        public required Major Major { get; set; }
    }
}
