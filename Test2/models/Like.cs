namespace Test2.models;

public class Like
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }
    public virtual Post Post { get; set; }
}