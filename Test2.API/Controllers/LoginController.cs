using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Test2.API.ViewModels.Requests;
using Test2.API.ViewModels.Responses;
using Test2.context;
using Test2.models;
using ClaimTypes = Test2.Extensions.common.consts.ClaimTypes;

namespace Test2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly DefaultDbContext _context;
        private readonly UserManager<User> _userManager;

        public LoginController(IConfiguration config, DefaultDbContext context, UserManager<User> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }
        
        [HttpGet]
        [Route("TestAuth")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> TestAuth()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var user = await Authenticate(request);
            if (user == null) return NotFound();

            var token = await GenerateToken(user);
            var accessToken = new AuthResponse()
            {
                accessToken = token
            };
            return Ok(accessToken);
        }

        private async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.UserName, user.FirstName),
                new Claim(ClaimTypes.UserEmail, user.Email!),
                new Claim(ClaimTypes.UserId, user.Id.ToString()),
                new Claim(ClaimTypes.UserRole, userRoles.First()),
            };
            var accessToken = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(accessToken);
            return token;
        }
        

        private async Task<User> Authenticate(AuthRequest userLogin)
        {
            var passwordHasher = new PasswordHasher<User>();
            var currentUser =
                await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == userLogin.Email.ToLower());

            if (currentUser == null) return null;
            
            var isPasswordEquals = passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, userLogin.Password);
            if (isPasswordEquals == PasswordVerificationResult.Success)
            {
                return currentUser;
            }

            return null;
        }
    }
}
