namespace Test2.API.ViewModels.Requests;

public class ModifyPostRequest
{
    public Guid PostId { get; set; }
    public string PostTitle { get; set; }
    public string PostText { get; set; }
}