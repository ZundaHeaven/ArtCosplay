using ArtCosplay.Data.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArtCosplay.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveMaxLength(200);

            base.ConfigureConventions(configurationBuilder);
        }

        public AppDbContext() => Database.EnsureCreated();

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

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");

            modelBuilder.Entity<User>().HasIndex(u => new { u.UserName, u.Email }).IsUnique();

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserName)
                    .HasMaxLength(20);

                entity.Property(u => u.Email)
                    .HasMaxLength(100);
            });

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

            modelBuilder.Entity<Discussion>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Discussion)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.Events)
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.HasOne(e => e.Author)
                    .WithMany(u => u.News)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(e => e.Seller)
                    .WithMany(u => u.Products)
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasOne(e => e.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Comment)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Post)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Discussion)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(e => e.DiscussionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Discussion>(entity =>
            {
                entity.HasOne(d => d.Author)
                  .WithMany(u => u.Discussions)
                  .HasForeignKey(d => d.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(d => d.Comments)
                      .WithOne(c => c.Discussion)
                      .HasForeignKey(c => c.DiscussionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(d => d.Likes)
                      .WithOne(l => l.Discussion)
                      .HasForeignKey(l => l.DiscussionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}