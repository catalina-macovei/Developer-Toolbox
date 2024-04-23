using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public DateTime? Date { get; set; }

        public int LikesNr { get; set; }

        public int DislikesNr { get; set; }

        [Required(ErrorMessage = "The content field is required!")]
        public string Content { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? QuestionId { get; set; }
        public virtual Question Question { get; set; }

    }
}
