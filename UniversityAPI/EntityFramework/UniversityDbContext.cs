using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityAPI.Entity;
using UniversityAPI.Models;

namespace UniversityAPI.EntityFramework
{
    public class UniversityDbContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole, string>(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestAnswer> TestAnswers { get; set; }
        public DbSet<TestResult> TestAnswersResults { get; set; }
        public DbSet<ScheduleElement> ScheduleElements { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherProfile> TeachersProfiles { get; set; }
        public DbSet<TeacherGroupSubject> TeacherGroupSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => rt.Token);

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasKey(tgs => new { tgs.TeacherProfileId, tgs.GroupId, tgs.SubjectId });

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.TeacherProfile)
                .WithMany(tp => tp.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.TeacherProfileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.Subject)
                .WithMany(s => s.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherGroupSubject>()
                .HasOne(tgs => tgs.Group)
                .WithMany(g => g.TeacherGroupSubjects)
                .HasForeignKey(tgs => tgs.GroupId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
