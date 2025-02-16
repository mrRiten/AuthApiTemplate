using AuthApiTemplate.Entity;
using AuthApiTemplate.Models;
using AuthApiTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthorizeService authorizeService,
        ProducerRabbitService<UserLogin> producerService, AuthApplicationContext context) : ControllerBase
    {
        private readonly IAuthorizeService _authorizeService = authorizeService;
        private readonly ProducerRabbitService<UserLogin> _producerService = producerService;
        private readonly AuthApplicationContext _context = context;

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
            if (!ModelState.IsValid) { return BadRequest("Unfull Data"); };

            await _authorizeService.Register(userLogin);
            await _producerService.SendMessageAsync(userLogin);

            return Ok();
        }

        [HttpPost("check-date")]
        public IActionResult CheckCreateDate([FromBody] IEnumerable<User> inputUsers)
        {
            var dateList = inputUsers.Select(u => u.CreateDate).ToList();

            var matchedUsers = _context.Users
                .Where(input => dateList.Contains(input.CreateDate))
                .ToList();

            return Ok();
        }
    }
}
