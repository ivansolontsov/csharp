namespace Test2.models;

public class Friend
{
    public Guid Id { get; set; }
    public Guid CurrentUserId { get; set; }
    public Guid FriendUserId { get; set; }
    
    public virtual User CurrentUser { get; set; }
    public virtual User FriendUser { get; set; }
}