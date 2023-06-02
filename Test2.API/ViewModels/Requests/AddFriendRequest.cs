namespace Test2.API.ViewModels.Requests;

public class AddFriendRequest
{
    public Guid CurrentUserId { get; set; }
    public Guid FriendUserId { get; set; }
}