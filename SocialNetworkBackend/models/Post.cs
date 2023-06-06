namespace Test2.models;

public class Post
{
    public Post()
    {
        Likes = new HashSet<Like>();
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PostTitle { get; set; }
    public string PostText { get; set; }
    public string PostCreatedDate { get; set; }
    
    public ICollection<Like> Likes { get; set; }
    public virtual User User { get; set; }
    
}