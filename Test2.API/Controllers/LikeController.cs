using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test2.API.ViewModels.Requests;
using Test2.context;
using Test2.models;

namespace Test2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly DefaultDbContext _context;

        public LikeController(DefaultDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        [Route("LikePost")]
        public async Task<IActionResult> LikePost([FromBody] LikePostRequest request)
        {
            var isUserExists = await _context.Users.AnyAsync(user => user.Id.ToString() == request.UserId.ToString());
            if (!isUserExists) return BadRequest("Зарегистрируйтесь, чтобы оставлять лайки");
            var isPostExists = await _context.Posts.AnyAsync(post => post.Id.ToString() == request.PostId.ToString());
            if (!isPostExists) return NotFound("Пост не найден");

            var dbLike = await _context.Likes.FirstOrDefaultAsync(like =>
                like.PostId.ToString() == request.PostId.ToString() &&
                like.UserId.ToString() == request.UserId.ToString());

            if (dbLike != null)
            {
                _context.Likes.Remove(dbLike);
                await _context.SaveChangesAsync();
                return Ok("Лайк удален");
            }
            
            var like = new Like()
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = request.UserId
            };

            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();

            return Ok("Post has been liked");
        }
    }
}
