using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public IActionResult Login([FromBody]LoginViewModel login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if(user != null)
            {
                var tokenString = GenerateJsonWebToken(user);

                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJsonWebToken(LoginViewModel login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Username)

            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], 
                                            _config["Jwt:Audience"],
                                            claims,
                                            notBefore: DateTime.Now,
                                            expires: DateTime.Now.AddMinutes(30),
                                            signingCredentials: credential);


            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        private LoginViewModel AuthenticateUser(LoginViewModel login)
        {
            var user = _userManager.FindByNameAsync(login.Username);
            LoginViewModel verifiedUser = null;

            if(user == null)
            {
                return null;
            } else
            {
                verifiedUser = new LoginViewModel { Username = login.Username };
            }

            return verifiedUser;
        }
    }
}