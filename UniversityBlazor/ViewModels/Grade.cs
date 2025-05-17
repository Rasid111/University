namespace UniversityBlazor.ViewModels
{
    public class Grade
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public required int Value { get; set; }
        public int SubjectId { get; set; }
        public int? StudentProfileId { get; set; }
        public int? TeacherProfileId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}