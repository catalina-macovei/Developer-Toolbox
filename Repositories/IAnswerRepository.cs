using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface IAnswerRepository
    {
        IEnumerable<Answer> GetAllAnswers();
        Answer GetAnswerById(int? id);
    }
}
