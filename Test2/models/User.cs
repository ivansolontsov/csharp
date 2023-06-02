using Microsoft.AspNetCore.Identity;

namespace Test2.models;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Posts = new HashSet<Post>();
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public ICollection<Like> Likes { get; set; }
    public ICollection<Post> Posts { get; set; }
    
    public ICollection<Friend> CurrentUserFriends { get; set; }
    
    public ICollection<Friend> FriendUserFriends { get; set; }
    
    public ICollection<FriendRequest>  UserFriendRequests { get; set; }
    
    public ICollection<FriendRequest> ReceiverFriendRequests { get; set; }
}