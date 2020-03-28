using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.Services;
using ZaporArrowAPI.ViewModels;

namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginViewModel login)
        {
            IActionResult response = Unauthorized();
            ApplicationUser user = await AuthenticateUser(login);

            if(user != null)
            {
                var tokenString = GenerateJsonWebToken(user);

                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJsonWebToken(ApplicationUser login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var user =  _userManager.FindByNameAsync(login.UserName);


            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.UserName),
                new Claim(ClaimTypes.Role, "Admin")

            };


            
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], 
                                            _config["Jwt:Audience"],
                                            claims,
                                            notBefore: DateTime.Now,
                                            expires: DateTime.Now.AddMinutes(30),
                                            signingCredentials: credential);


            return   new JwtSecurityTokenHandler().WriteToken(token);


        }

        private async Task<ApplicationUser> AuthenticateUser(LoginViewModel login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);

            return user;
        }

        private void AddCustomRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}