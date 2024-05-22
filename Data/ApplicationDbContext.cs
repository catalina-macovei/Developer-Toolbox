using Developer_Toolbox.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Developer_Toolbox.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<QuestionTag>()
            .HasKey(c => new
            {
                c.Id,
                c.QuestionId,
                c.TagId
            });

            // definire relatii cu modelele Question si Tag (FK)
            modelBuilder.Entity<QuestionTag>()
            .HasOne(c => c.Tag)
            .WithMany(c => c.QuestionTags)
            .HasForeignKey(c => c.TagId);
            modelBuilder.Entity<QuestionTag>()
            .HasOne(c => c.Question)
            .WithMany(c => c.QuestionTags)
            .HasForeignKey(c => c.QuestionId);

            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<Bookmark>()
            .HasKey(c => new
            {
                c.Id,
                c.UserId,
                c.QuestionId
            });

            // definire relatii cu modelele User si Question (FK)
            modelBuilder.Entity<Bookmark>()
            .HasOne(c => c.Question)
            .WithMany(c => c.Bookmarks)
            .HasForeignKey(c => c.QuestionId);
            modelBuilder.Entity<Bookmark>()
            .HasOne(c => c.User)
            .WithMany(c => c.Bookmarks)
            .HasForeignKey(c => c.UserId);

            ///

            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity< Reaction>()
            .HasKey(c => new
            {
                c.Id,
                c.UserId,
                c.QuestionId
            });

            // definire relatii cu modelele User si Question (FK)
            modelBuilder.Entity<Reaction>()
            .HasOne(c => c.Question)
            .WithMany(c => c.Reactions)
            .HasForeignKey(c => c.QuestionId);
            modelBuilder.Entity<Reaction>()
            .HasOne(c => c.User)
            .WithMany(c => c.Reactions)
            .HasForeignKey(c => c.UserId);
        }
    }
}