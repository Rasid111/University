using Microsoft.EntityFrameworkCore;
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
    }
}
