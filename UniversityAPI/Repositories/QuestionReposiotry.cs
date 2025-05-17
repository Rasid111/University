using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class QuestionRepository(UniversityDbContext context) : IBaseRepository<Question>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(Question entity)
        {
            await _context.Questions.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.Questions.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<Question>> Get()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question?> Get(int id)
        {
            return await _context.Questions.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(Question entity)
        {
            _context.Questions.Update(entity);
            _context.SaveChanges();
        }
    }
}
