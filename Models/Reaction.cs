namespace Developer_Toolbox.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public int? QuestionId { get; set; }
        public virtual Question? Question { get; set; }

        public bool? Liked { get; set; } = false;

        public bool? Disliked { get; set; } = false;
    }
}
