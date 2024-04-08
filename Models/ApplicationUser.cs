using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Developer_Toolbox.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Exercise>? Exercises { get; set; }
        public virtual ICollection<Solution>? Solutions { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
