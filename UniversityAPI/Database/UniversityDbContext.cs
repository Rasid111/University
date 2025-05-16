using Microsoft.EntityFrameworkCore;
using UniversityAPI.Models;

namespace UniversityAPI.Database
{
    public class UniversityDbContext(DbContextOptions<UniversityDbContext> options) : DbContext(options)
    {
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherGroupSubject> TeacherGroupSubjects { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<TestResult> TestResults { get; set; }

        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.TeacherProfile)
                .WithMany(t => t.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.TeacherProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.Group)
                .WithMany(g => g.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.Subject)
                .WithMany(s => s.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherProfile>()
                .HasMany(t => t.Groups)
                .WithMany(g => g.TeacherProfiles)
                .UsingEntity<TeacherGroupSubject>();

            modelBuilder.Entity<TeacherProfile>()
                .HasMany(t => t.Subjects)
                .WithMany(s => s.TeacherProfiles)
                .UsingEntity<TeacherGroupSubject>();

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Subjects)
                .WithMany(s => s.Groups)
                .UsingEntity<TeacherGroupSubject>();
        }
    }
}
