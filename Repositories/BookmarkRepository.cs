using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using System.Collections.Generic;
using System.Linq;
using Developer_Toolbox.Repositories;

namespace Developer_Toolbox.Repositories
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly ApplicationDbContext _context;

        public BookmarkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Bookmark> GetAllBookmarks()
        {
            return _context.Bookmarks.ToList();
        }

        public Bookmark GetBookmarkById(int? id)
        {
            return _context.Bookmarks.FirstOrDefault(b => b.Id == id);
        }
    }
}
