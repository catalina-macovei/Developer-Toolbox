using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAllTags();
        Tag GetTagById(int? id);
    }
}
