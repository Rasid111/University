
namespace UniversityAPI.Dtos
{
    public class DegreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateDegreeDto
    {
        public string Name { get; set; }
    }

    public class UpdateDegreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
