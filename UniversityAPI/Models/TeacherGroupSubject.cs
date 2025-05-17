namespace UniversityAPI.Models
{
    public class TeacherGroupSubject
    {
        public int Id { get; set; }
        public int Classroom { get; set; }

        public List<ScheduleElement> Schedule { get; set; } = [];
        public int TeacherProfileId { get; set; }
        public required TeacherProfile TeacherProfile { get; set; }
        public int GroupId { get; set; }
        public required Group Group { get; set; }
        public int SubjectId { get; set; }
        public required Subject Subject { get; set; }
    }
}
