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

        public virtual ICollection<Solution>? Solutions { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public string? Title { get; set; }
        public string?  Description { get; set; }
        public DateTime? Date { get; set; }
        public string Summary { get; set; }
        public string Restrictions { get; set; }
        public string Examples { get; set; }

        // file test

    }
}
