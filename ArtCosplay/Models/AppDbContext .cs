using ArtCosplay.Models.DB;
using Microsoft.EntityFrameworkCore;

using System.Reflection.Metadata;
public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=cosplay.db");
    }

    public AppDbContext() => Database.EnsureCreated();

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<DiscussionReply> DiscussionReplies { get; set; }
    public DbSet<AnimeCharacter> AnimeCharacters { get; set; }
    public DbSet<PostCharacter> PostCharacters { get; set; }

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
    }
}