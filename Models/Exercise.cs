using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Category required!")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
