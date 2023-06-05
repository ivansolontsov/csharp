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
using Test2.Extensions.common.enums;
using Test2.Extensions.common.helpers;
using Test2.models;

namespace Test2.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SocialController : ControllerBase
    {
        private readonly DefaultDbContext _context;

        public SocialController(DefaultDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("AddFriend")]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            var userId = HttpContext.GetCurrentUser();
            if (userId == request.FriendUserId) return BadRequest("Вы не можете добавить в друзья сами себя");
            
            var isUserInFriendList = await _context.Friends.AnyAsync(friend =>
                userId == friend.CurrentUserId && request.FriendUserId == friend.FriendUserId);
            if (isUserInFriendList) return BadRequest("Пользователь уже в списке друзей");
            
            var isUserInFriendRequestList = await _context.FriendRequests.AnyAsync(friend =>
                userId == friend.UserId && request.FriendUserId == friend.ReceiverId);
            if (isUserInFriendRequestList) return BadRequest("Заявка уже отправлена");

            var friendRequest = new FriendRequest()
            {
                UserId = userId,
                ReceiverId = request.FriendUserId,
            };

            await _context.FriendRequests.AddAsync(friendRequest);
            await _context.SaveChangesAsync();
            return Ok("Заявка в друзья отправлена");
        }
        
        [HttpGet]
        [Route("GetFriendRequests")]
        public async Task<IActionResult> GetFriendRequests([FromQuery] Guid id)
        {
            var friendRequestsList = await _context.FriendRequests.Where(item => item.UserId == id).ToListAsync();
            return Ok(friendRequestsList);
        }

        [HttpPost]
        [Route("FriendAddOrDeclineRequest")]
        public async Task<IActionResult> FriendAddOrDecline([FromBody] FriendActionRequest request)
        {
            var userId = HttpContext.GetCurrentUser();
            var friendRequest =
                await _context.FriendRequests.FirstOrDefaultAsync(item => item.Id == request.FriendRequestId);
            if (friendRequest == null) return NotFound("Заявка не найдена");
            if (userId != friendRequest.ReceiverId) return BadRequest("Вы не можете подтверждать чужие заявки");
            
            if (request.ActionType == (int)FriendActions.Add)
            {
                var friend = new Friend()
                {
                    Id = Guid.NewGuid(),
                    CurrentUserId = friendRequest.UserId,
                    FriendUserId = friendRequest.ReceiverId
                };
                await _context.Friends.AddAsync(friend);
                await _context.SaveChangesAsync();
                return Ok("Заявка в друзья одобрена");
            }
            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
    }
}
//id(request.action == (int)FriendActions.Add){
