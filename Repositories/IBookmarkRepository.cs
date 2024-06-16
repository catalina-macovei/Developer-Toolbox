using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface IBookmarkRepository
    {
        IEnumerable<Bookmark> GetAllBookmarks();
        Bookmark GetBookmarkById(int? id);
    }
}
