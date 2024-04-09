namespace Developer_Toolbox.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? QuestionId { get; set; }
        public virtual Question? Question { get; set; }
    }
}
