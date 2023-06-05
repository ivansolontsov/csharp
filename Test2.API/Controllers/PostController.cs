using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test2.API.ViewModels.Requests;
using Test2.context;
using Test2.Extensions.common.helpers;
using Test2.Extensions.mappers;
using Test2.models;

namespace Test2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : ControllerBase
    {
        private readonly DefaultDbContext _context;

        public PostController(DefaultDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var userId = HttpContext.GetCurrentUser();
            if (userId == Guid.Empty) return Unauthorized();
            var post = new Post()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PostCreatedDate = DateTime.Now.ToString(),
                PostTitle = request.PostTitle,
                PostText = request.PostText
            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return Ok("");
        }

        [HttpGet]
        [Route("GetPostById")]
        public async Task<IActionResult> GetPostById([FromQuery] Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(item => item.Id == id);
            if (post == null) return NotFound("Пост не найден");

            return Ok(post);
        }
        
        [HttpGet]
        [Route("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var postList = await _context.Posts
                .Include(item => item.User)
                .Include(item => item.Likes)
                .ToListAsync();
            var mapPosts = PostMapper.MapByListProduct(postList);
            return Ok(mapPosts);
        }

        [HttpDelete]
        [Route("DeletePostById")]
        public async Task<IActionResult> DeletePostById([FromQuery] string id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(post => post.Id.ToString() == id);
            if (post == null) return NotFound("Пост не найден");
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok("Пост удален");
        }
        
        [HttpPatch]
        [Route("ModifyPost")]
        public async Task<IActionResult> ModifyPost([FromBody] ModifyPostRequest request)
        {
            var userId = HttpContext.GetCurrentUser();
            var post = await _context.Posts
                .FirstOrDefaultAsync(post => post.Id.ToString() == request.PostId.ToString() && post.UserId == userId);
            if (post == null) return NotFound("Пост не найден");

            post.PostText = request.PostText;
            post.PostTitle = request.PostTitle;
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}
