using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetAllQuestions();
        Question GetQuestionById(int? id);
    }
}
