using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Solution
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<Exercise>? Exercises { get; set; }
    }
}
