using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface IExerciseRepository
    {
        IEnumerable<Exercise> GetAllExercises();
        Exercise GetExerciseById(int? id);
    }
}