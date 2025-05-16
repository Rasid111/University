
namespace UniversityAPI.Dtos
{
    public class StudentProfileDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class CreateStudentProfileDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
    }

    public class UpdateStudentProfileDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
    }
}
