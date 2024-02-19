using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using proiectasp.Models;

namespace proiectasp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FollowList> FollowLists { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<GroupPost> GroupPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            modelBuilder.Entity<GroupPost>()
            .HasKey(ab => new {ab.Id,ab.GroupId,ab.PostId});


            // definire relatii cu modelele Post si Group (FK)

            modelBuilder.Entity<GroupPost>()
                .HasOne(ab => ab.Post)
                .WithMany(ab => ab.GroupPosts)
                .HasForeignKey(ab => ab.PostId);
            modelBuilder.Entity<GroupPost>()
                .HasOne(ab => ab.Group)
                .WithMany(ab => ab.GroupPosts)
                .HasForeignKey(ab => ab.GroupId);
        }
    }

}
    