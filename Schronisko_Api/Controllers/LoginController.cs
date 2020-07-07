using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Schronisko_Api.DTOs;
using Schronisko_Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Schronisko_Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private ShelterDbContext _context;

        public LoginController(ShelterDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [EnableCors("developerska")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            try
            {
                User findUser = _context.Users.Where(x => x.Login == user.Login && x.Password == user.Password).Single();

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, findUser.Login),
                        new Claim(ClaimTypes.Role, findUser.Role)
                    };


                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:44368",
                    audience: "https://localhost:44368",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { token = tokenString, role = findUser.Role });


            }
            catch
            {
                return Unauthorized();
            }

        }
    }
}
