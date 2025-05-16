
namespace UniversityAPI.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public int MajorId { get; set; }
        public string MajorName { get; set; }
        public List<string> SubjectNames { get; set; }
        public List<string> TeacherNames { get; set; }
        public List<string> StudentNames { get; set; }
    }

    public class CreateGroupDto
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int FacultyId { get; set; }
        public int MajorId { get; set; }
    }

    public class UpdateGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int FacultyId { get; set; }
        public int MajorId { get; set; }
    }
}
