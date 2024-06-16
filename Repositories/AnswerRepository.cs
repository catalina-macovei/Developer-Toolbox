using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;

namespace Developer_Toolbox.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Answer> GetAllAnswers()
        {
            return _context.Answers.ToList();
        }

        public Answer GetAnswerById(int? id)
        {
            return _context.Answers.FirstOrDefault(a => a.Id == id);
        }
    }
}
