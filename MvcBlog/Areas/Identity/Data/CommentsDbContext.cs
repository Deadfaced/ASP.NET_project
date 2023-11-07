using Microsoft.EntityFrameworkCore;
using MvcBlog.Models;

namespace MvcBlog.Data
{
    public class CommentsDbContext : DbContext
    {
        public CommentsDbContext(DbContextOptions<CommentsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
    }
}