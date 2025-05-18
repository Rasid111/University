using Microsoft.EntityFrameworkCore;
using UniversityAPI.EntityFramework;
using UniversityAPI.Models;
using UniversityAPI.Repositories.Base;

namespace UniversityAPI.Repositories
{
    public class QuestionAnswerRepository(UniversityDbContext context) : IBaseRepository<QuestionAnswer>
    {
        readonly UniversityDbContext _context = context;
        public async Task<int> Create(QuestionAnswer entity)
        {
            await _context.QuestionAnswers.AddAsync(entity);
            _context.SaveChanges();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await Get(id) ?? throw new KeyNotFoundException();
            _context.QuestionAnswers.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<QuestionAnswer>> Get()
        {
            return await _context.QuestionAnswers.ToListAsync();
        }

        public async Task<QuestionAnswer?> Get(int id)
        {
            return await _context.QuestionAnswers.FirstOrDefaultAsync(d => d.Id == id);
        }

        public void Update(QuestionAnswer entity)
        {
            _context.QuestionAnswers.Update(entity);
            _context.SaveChanges();
        }
    }
}
