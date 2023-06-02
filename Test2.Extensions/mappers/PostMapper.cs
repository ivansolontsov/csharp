using AutoMapper;
using Test2.models;
using Test2.ViewModels;

namespace Test2.Extensions.mappers;

public class PostMapper
{
    public static List<PostViewModel> MapByListProduct(List<Post> dbProducts)
    {
        return dbProducts.Select(CheckById).ToList();
    }

    public static PostViewModel CheckById(Post dbPost)
    {
        var post = new PostViewModel();

        var config = new MapperConfiguration(cfg => cfg.CreateMap<Post, PostViewModel>()
            .ForMember(item => item.Author, 
                opt => 
                    opt.MapFrom(item => 
                        new UserViewModel()
                            {
                                Id = item.User.Id,
                                FirstName = item.User.FirstName,
                                LastName = item.User.LastName
                            }
                        )
                    )
            .ForMember(item => item.Likes, 
                opt => opt.MapFrom(item => item.Likes.Select(item => new LikeVIewModel()
                {
                    Id = item.Id,
                    User = new UserViewModel()
                    {
                        Id = item.User.Id,
                        FirstName = item.User.FirstName,
                        LastName = item.User.LastName
                    }
                })))
        );
        
        var mapper = new AutoMapper.Mapper(config);
        post = mapper.Map<Post, PostViewModel>(dbPost);
        return post;
    }
}