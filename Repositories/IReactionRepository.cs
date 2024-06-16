using System.Collections.Generic;
using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface IReactionRepository
    {
        IEnumerable<Reaction> GetAllReactions();
        Reaction GetReactionById(int? id);
    }
}
