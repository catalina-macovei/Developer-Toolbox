using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface ISolutionRepository
    {
        IEnumerable<Solution> GetAllSolutions();
        Solution GetSolutionById(int? id);
    }
}