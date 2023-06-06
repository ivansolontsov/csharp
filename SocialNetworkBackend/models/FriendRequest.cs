namespace Test2.models;

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ReceiverId { get; set; }
    
    public virtual User User { get; set; }
    public virtual User Receiver { get; set; }
}