using MvcBlog.Areas.Identity.Data;
using MvcBlog.Models;

public class UserLike
{
    public int Id { get; set; } // Primary key
    public string UserId { get; set; } // Foreign key for User
    public int PostId { get; set; } // Foreign key for Post
    public bool IsLike { get; set; } // True for like, false for dislike

    // Navigation properties
    public MvcBlogUser User { get; set; }
    public PostModel Post { get; set; }
}