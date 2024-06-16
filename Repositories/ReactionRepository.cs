using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using Developer_Toolbox.Repositories;


namespace Developer_Toolbox.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Reaction> GetAllReactions()
        {
            return _context.Reactions.ToList();
        }

        public Reaction GetReactionById(int? id)
        {
            return _context.Reactions.FirstOrDefault(b => b.Id == id);
        }
    }
}
