using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;

namespace Developer_Toolbox.Repositories
{
    public class SolutionRepository : ISolutionRepository
    {
        private readonly ApplicationDbContext _context;

        public SolutionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Solution> GetAllSolutions()
        {
            return _context.Solutions.ToList();
        }

        public Solution GetSolutionById(int? id)
        {
            return _context.Solutions.FirstOrDefault(s => s.Id == id);
        }
    }
}