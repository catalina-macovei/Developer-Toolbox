using Developer_Toolbox.Models;

namespace Developer_Toolbox.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        Category GetCategoryById(int? id);
    }
}