using ArtCosplay.Data.DB;
using Microsoft.EntityFrameworkCore;
namespace ArtCosplay.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public AppDbContext() => Database.EnsureCreated();

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<News> News => Set<News>(); 
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Discussion> Discussions => Set<Discussion>();  
        public DbSet<AnimeCharacter> AnimeCharacters => Set<AnimeCharacter>();
        public DbSet<PostCharacter> PostCharacters => Set<PostCharacter>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => new { u.Username, u.Email }).IsUnique();

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();

            modelBuilder.Entity<PostCharacter>()
                .HasIndex(pc => new { pc.PostId, pc.CharacterId })
                .IsUnique();

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.Cascade);

             modelBuilder!.Entity<Comment>()
                .HasCheckConstraint(
                    "CK_Comment_Parents",
                    "CASE WHEN PostId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN DiscussionId IS NOT NULL THEN 1 ELSE 0 END = 1"
                );

            modelBuilder.Entity<Like>()
                .HasCheckConstraint(
                    "CK_Like_Targets",
                    "CASE WHEN PostId IS NOT NULL THEN 1 ELSE 0 END + " +
                    "CASE WHEN CommentId IS NOT NULL THEN 1 ELSE 0 END + " +
                    "CASE WHEN DiscussionId IS NOT NULL THEN 1 ELSE 0 END + "
                );
        }
    }
}