using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class SubjectRepository(UniversityDbContext context) : IBaseRepository<Subject>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Subject entity)
        {
            await _context.Subjects.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Subjects.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Subject>> Get()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject?> Get(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<List<Subject>> GetByUserId(int id)
        {
            return await _context.Subjects
                .Include(s => s.Tests)
                .ThenInclude(t => t.Questions)
                .Include(s => s.TeacherGroupSubjects)
                .ThenInclude(tgs => tgs.Group)
                .ThenInclude(g => g.Students)
                .Where(s => s.TeacherGroupSubjects
                    .Any(tgs => tgs.Group.Students
                        .Any(s => s.Id == id)))
                .ToListAsync();
        }
        public void Update(Subject entity)
        {
            _context.Subjects.Update(entity);
            _context.SaveChanges();
        }
    }
}
