using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the category is mandatory")]
        public string? CategoryName { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "The logo of the category is mandatory")]
        public string? Logo { get; set; } = "/images/categories/default.png";
        public virtual ICollection<Exercise>? Exercises { get; set; }
    }
}
