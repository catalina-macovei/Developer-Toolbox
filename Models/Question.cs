using System.ComponentModel.DataAnnotations;

namespace Developer_Toolbox.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public virtual ICollection<QuestionTag>? QuestionTags { get; set; }

        public virtual ICollection<Answer>? Answers { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }

        public virtual ICollection<Reaction>? Reactions { get; set; }

        [Required(ErrorMessage = "The title field is required!")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters")]
        [MinLength(5, ErrorMessage = "The title must be at least 5 characters long")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The description field is required!")]
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? LikesNr {  get; set; }
        public int? DislikesNr { get; set; }
    }
}
