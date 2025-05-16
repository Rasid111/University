
namespace UniversityAPI.Dtos
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateSubjectDto
    {
        public string Name { get; set; }
    }

    public class UpdateSubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
