using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApiTemplate.Entity;
using AuthApiTemplate.Helpers;
using AuthApiTemplate.Models;
using AuthApiTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthorizeService authorizeService) : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService = authorizeService;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            var responseTokens = await _authorizeService.SingIn(userLogin);

            if (responseTokens == null)
            {
                return BadRequest("Invalid user data");
            }

            return Ok(responseTokens);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromHeader] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("No Token");
            }

            var responseTokens = await _authorizeService.RefreshToken(refreshToken);

            if (responseTokens == null)
            {
                return BadRequest("Invalid token");
            }

            return Ok(responseTokens);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserLogin userLogin)
        {
            await _authorizeService.Register(userLogin);

            if (!ModelState.IsValid) { return BadRequest("Unfull Data"); }


            return Ok();
        }
    }
}
