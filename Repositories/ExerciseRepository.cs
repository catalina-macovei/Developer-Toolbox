using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;

namespace Developer_Toolbox.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Exercise> GetAllExercises()
        {
            return _context.Exercises.ToList();
        }

        public Exercise GetExerciseById(int? id)
        {
            return _context.Exercises.FirstOrDefault(e => e.Id == id);
        }
    }
}