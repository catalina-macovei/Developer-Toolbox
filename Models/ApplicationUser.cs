using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Developer_Toolbox.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? ReputationPoints { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Exercise>? Exercises { get; set; }
        public virtual ICollection<Solution>? Solutions { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }

        public virtual ICollection<Answer>? Answers { get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
