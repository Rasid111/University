
namespace UniversityAPI.Dtos
{
    public class MajorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateMajorDto
    {
        public string Name { get; set; }
    }

    public class UpdateMajorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
