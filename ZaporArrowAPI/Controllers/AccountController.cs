using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //public IActionResult Authentication()
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
        //        new Claim("Granny", "cookie"),
        //    };

        //    var token = new JwtSecurityToken();

        
    }
}