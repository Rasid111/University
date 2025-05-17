namespace UniversityApplication.Dtos
{
    public class TeacherProfileCreateDto
    {
        public required string UserId { get; set; }
        public required int FacultyId { get; set; }
        public required int DegreeId { get; set; }
    }
}
