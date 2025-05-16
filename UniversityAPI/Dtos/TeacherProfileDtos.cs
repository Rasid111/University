
namespace UniversityAPI.Dtos
{
    public class TeacherProfileDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int DegreeId { get; set; }
        public string DegreeName { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public List<string> SubjectNames { get; set; }
        public List<string> GroupNames { get; set; }
    }

    public class CreateTeacherProfileDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int DegreeId { get; set; }
        public int FacultyId { get; set; }
    }

    public class UpdateTeacherProfileDto
    {
        public int Id { get; set; }
        public int DegreeId { get; set; }
        public int FacultyId { get; set; }
    }
}
