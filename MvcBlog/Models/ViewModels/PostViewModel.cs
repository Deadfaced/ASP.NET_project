namespace MvcBlog.Models.ViewModels;

public class PostViewModel
{
    public List<PostModel>? PostList { get; set; }

    public PostModel? Post { get; set; }

    public Comment NewComment { get; set; }

    public List<Comment> Comments { get; set; }

    public PostViewModel()
    {
        NewComment = new Comment();
    }
}
