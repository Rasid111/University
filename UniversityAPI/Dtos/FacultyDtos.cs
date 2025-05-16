// File: Dtos/FacultyDtos.cs

namespace UniversityAPI.Dtos
{
    public class FacultyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateFacultyDto
    {
        public string Name { get; set; }
    }

    public class UpdateFacultyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
