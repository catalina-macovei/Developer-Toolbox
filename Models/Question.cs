using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<QuestionTag>? QuestionTags { get; set; }
    }
}
