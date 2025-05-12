namespace UniversityAPI.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public int GroupId { get; set; }
        public required Group Group { get; set; }
    }
}