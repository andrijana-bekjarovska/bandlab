using Imagegram.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Data
{
    public class ImagegramContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        
        public ImagegramContext(DbContextOptions options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(i => i.Account)
                .WithMany(i => i.Posts)
                .HasForeignKey(i => i.Creator);

            modelBuilder.Entity<Comment>()
                .HasOne(i => i.Account)
                .WithMany(i => i.Comments)
                .HasForeignKey(i => i.Creator);

            modelBuilder.Entity<Post>()
                .HasMany(i => i.Comments)
                .WithOne(i => i.Post)
                .HasForeignKey(i => i.PostId);
        }
    }
}