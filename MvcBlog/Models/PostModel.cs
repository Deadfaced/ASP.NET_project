using System.ComponentModel.DataAnnotations.Schema;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Models;

[Table("Post")]
public class PostModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; }

    public PostModel()
    {
        Comments = new List<Comment>();
    }

    public int Likes { get; set; }
    
    public int Dislikes { get; set; }
}
