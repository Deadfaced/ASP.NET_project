using Microsoft.EntityFrameworkCore;
using MvcBlog.Models;

namespace MvcBlog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<UserLike> UserLikes { get; set; }

        public DbSet<PostModel> Posts { get; set; }
    }
}