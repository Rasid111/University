namespace UniversityApplication.Dtos
{
    public class GradeBoxDto
{
    public int Value { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
}
}
