namespace Test2.API.ViewModels.Requests;

public class LikePostRequest
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}