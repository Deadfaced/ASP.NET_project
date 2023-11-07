using MvcBlog.Models.ViewModels;

namespace MvcBlog.Models;

public class Comment
{
    public int Id { get; set; }

    public string Text { get; set; }

    public string Author { get; set; }

    public int PostId { get; set; }

    public PostModel Post { get; set; }
}