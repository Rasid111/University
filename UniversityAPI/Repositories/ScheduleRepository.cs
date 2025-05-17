using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class ScheduleRepository(UniversityDbContext context) : IBaseRepository<ScheduleElement>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(ScheduleElement entity)
        {
            await _context.ScheduleElements.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.ScheduleElements.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<ScheduleElement>> Get()
        {
            return await _context.ScheduleElements.ToListAsync();
        }

        public async Task<ScheduleElement?> Get(int id)
        {
            return await _context.ScheduleElements.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(ScheduleElement entity)
        {
            _context.ScheduleElements.Update(entity);
            _context.SaveChanges();
        }
    }
}
