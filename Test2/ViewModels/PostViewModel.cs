using Test2.models;

namespace Test2.ViewModels;

public class PostViewModel
{
    public Guid Id { get; set; }
    public string PostTitle { get; set; }
    public string PostText { get; set; }
    public string PostCreatedDate { get; set; }
    
    public List<LikeVIewModel> Likes { get; set; }
    public UserViewModel Author { get; set; }
}