using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class TeacherGroupSubjectRepository(UniversityDbContext context) : IBaseRepository<TeacherGroupSubject>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(TeacherGroupSubject entity)
        {
            await _context.TeacherGroupSubjects.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task<List<TeacherGroupSubject>> GetScheduleByGroupId(int id)
        {
            var res = await _context.TeacherGroupSubjects
                .Include(tgs => tgs.Schedule)
                .Include(tgs => tgs.TeacherProfile)
                .ThenInclude(tgs => tgs.User)
                .Include(tgs => tgs.Subject)
                .Where(tgs => tgs.GroupId == id)
                .ToListAsync();

            return res;
        }
        public async Task<List<TeacherGroupSubject>> GetScheduleByTeacherId(int id)
        {
            var res = await _context.TeacherGroupSubjects
                .Include(tgs => tgs.Schedule)
                .Include(tgs => tgs.Group)
                .Include(tgs => tgs.Subject)
                .Where(tgs => tgs.TeacherProfileId == id)
                .ToListAsync();

            return res;
        }
        
        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.TeacherGroupSubjects.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<TeacherGroupSubject>> Get()
        {
            return await _context.TeacherGroupSubjects.ToListAsync();
        }

        public async Task<TeacherGroupSubject?> Get(int id)
        {
            return await _context.TeacherGroupSubjects
                .Include(tgs => tgs.Schedule)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<List<TeacherGroupSubject>> GetByGroup(int groupId)
        {
            return await _context.TeacherGroupSubjects
                .Include(tgs => tgs.Schedule)
                .Where(d => d.GroupId == groupId)
                .ToListAsync();
        }
        public void Update(TeacherGroupSubject entity)
        {
            _context.TeacherGroupSubjects.Update(entity);
            _context.SaveChanges();
        }

        public async Task<List<Group>> GetGroupsByTeacherId(int teacherId)
        {
            return await _context.TeacherGroupSubjects
                .Where(x => x.TeacherProfileId == teacherId)
                .Include(x => x.Group)
                    .ThenInclude(g => g.Students)
                .Include(x => x.Group)
                    .ThenInclude(g => g.Faculty)
                .Include(x => x.Group)
                    .ThenInclude(g => g.Major)
                .Select(x => x.Group)
                .ToListAsync();
        }

    }
}
