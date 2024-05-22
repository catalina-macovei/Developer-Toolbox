using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Developer_Toolbox.Models
{
    public class ApplicationUser : IdentityUser
    {
        //[Required(ErrorMessage = "First name is required")]
       // [StringLength(100, ErrorMessage = "First name cannot have more than 100 characters")]
        public string? FirstName { get; set; }
        //[Required(ErrorMessage = "Last name is required")]
        //[StringLength(100, ErrorMessage = "Last name cannot have more than 100 characters")]
        public string? LastName { get; set; }
        public int? ReputationPoints { get; set; }
        public string? EmailAddress { get; set; }

       // [Required(ErrorMessage = "Data nașterii este obligatorie.")]
        public DateTime? Birthday { get; set; }

        //[Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }
        public virtual ICollection<Exercise>? Exercises { get; set; }
        public virtual ICollection<Solution>? Solutions { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }

        public virtual ICollection<Answer>? Answers { get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }
        public virtual ICollection<Reaction>? Reactions { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
