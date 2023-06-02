using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test2.API.ViewModels.Requests;
using Test2.context;
using Test2.models;

namespace Test2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DefaultDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(DefaultDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var isUserExists = await _context.Users.AnyAsync(item => item.Email.ToLower() == request.email.ToLower());
            if (isUserExists) return BadRequest("Пользователь с таким Email уже существует");

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = request.email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("Пользователь успешно создан");
        }

        [HttpGet]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Email.ToLower() == email.ToLower());
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var usersList = await _context.Users.Include(user => user.CurrentUserFriends).ToListAsync();
            return Ok(usersList);
        }
    }
}
