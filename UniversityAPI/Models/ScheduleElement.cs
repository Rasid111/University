namespace UniversityAPI.Models
{
    public class ScheduleElement
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        
        public int TeacherGroupSubjectId { get; set; }
        public required TeacherGroupSubject TeacherGroupSubject { get; set; }
    }
}
