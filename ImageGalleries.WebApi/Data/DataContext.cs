using ImageGalleries.WebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImageGalleries.WebApi.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<PictureGallery> PictureGalleries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PictureTag> PictureTags { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PictureGallery>()
                    .HasKey(pg => new { pg.PictureId, pg.GalleryId });
            modelBuilder.Entity<PictureGallery>()
                    .HasOne(pg => pg.Picture)
                    .WithMany(p => p.PictureGalleries)
                    .HasForeignKey(pg => pg.PictureId);
            modelBuilder.Entity<PictureGallery>()
                    .HasOne(pg => pg.Gallery)
                    .WithMany(g => g.PictureGalleries)
                    .HasForeignKey(pg => pg.GalleryId);

            modelBuilder.Entity<PictureTag>()
                    .HasKey(pt => new { pt.PictureId, pt.TagId });
            modelBuilder.Entity<PictureTag>()
                    .HasOne(pt => pt.Picture)
                    .WithMany(p => p.PictureTags)
                    .HasForeignKey(pt => pt.PictureId);
            modelBuilder.Entity<PictureTag>()
                    .HasOne(pt => pt.Tag)
                    .WithMany(t => t.PictureTags)
                    .HasForeignKey(pt => pt.TagId);

            modelBuilder.Entity<Score>()
                    .HasKey(s => new { s.PictureId, s.UserId });
            modelBuilder.Entity<Score>()
                    .HasOne(s => s.Picture)
                    .WithMany(p => p.Scores)
                    .HasForeignKey(s => s.PictureId);
            modelBuilder.Entity<Score>()
                    .HasOne(s => s.User)
                    .WithMany(u => u.Scores)
                    .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Comment>()
                    .HasKey(c => new { c.PictureId, c.UserId });
            modelBuilder.Entity<Comment>()
                    .HasOne(c => c.Picture)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PictureId);
            modelBuilder.Entity<Comment>()
                    .HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId);
        }
    }
}
