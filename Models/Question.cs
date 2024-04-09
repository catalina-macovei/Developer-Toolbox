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

        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? LikesNr {  get; set; }
        public int? DislikesNr { get; set; }
    }
}
