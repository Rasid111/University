using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class GroupRepository(UniversityDbContext context) : IBaseRepository<Group>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Group entity)
        {
            await _context.Groups.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Groups.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Group>> Get()
        {
              return await _context.Groups
                .Include(g => g.Students)
                .Include(g => g.Faculty)
                .Include(g => g.Major)
                .ToListAsync();
        }

        public async Task<Group?> Get(int id)
        {
            return await _context.Groups
                .Include(g => g.Students)
                    .ThenInclude(s => s.User)
                .Include(g => g.Faculty)
                .Include(g => g.Major)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public void Update(Group entity)
        {
            _context.Groups.Update(entity);
            _context.SaveChanges();
        }
    }
}
