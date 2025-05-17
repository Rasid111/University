namespace UniversityApplication.Dtos
{
    public class GradeCreateDto
    {
        public required int Value { get; set; }
        public string? Message { get; set; }
        public required int SubjectId { get; set; }
        public int? StudentProfileId { get; set; }
        public int? TeacherProfileId { get; set; }
        public DateTime? Date { get; set; }
    }
}
