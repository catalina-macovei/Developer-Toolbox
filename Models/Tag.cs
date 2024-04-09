using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<QuestionTag>? QuestionTags { get; set; }
        public string Name { get; set; }

    }
}
