using AuthApiTemplate.Entity;
using AuthApiTemplate.Helpers;
using AuthApiTemplate.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

namespace AuthApiTemplate.Services
{
    public interface IAuthorizeService
    {
        public Task<AuthTokenResponse?> SingIn(UserLogin userLogin);
        public Task Register(UserLogin userLogin);
        public Task<AuthTokenResponse?> RefreshToken(string oldRefreshToken);
    }

    public class AuthorizeService(AuthApplicationContext context, IJwtHelper jwtHelper) : IAuthorizeService
    {
        private readonly AuthApplicationContext _context = context;
        private readonly IJwtHelper _jwtHelper = jwtHelper;

        public async Task<AuthTokenResponse?> RefreshToken(string oldRefreshToken)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.RefreshToken == oldRefreshToken);

            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await SaveRefreshToken(user, refreshToken);

            return new AuthTokenResponse
            {
                AccessToken = _jwtHelper.GenerateJwt(user),
                RefreshToken = refreshToken
            };
        }

        public async Task Register(UserLogin userLogin)
        {
            var user = new User
            {
                UserName = userLogin.Username,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(userLogin.Password),
                Email = userLogin.Username,
                RoleId = (int)RoleNameEnum.User,
                ConfirmToken = "NoToken"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthTokenResponse?> SingIn(UserLogin userLogin)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.UserName == userLogin.Username || u.Email == userLogin.Username);

            //if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.HashPassword))
            //{
            //    return null;
            //}

            var refreshToken = _jwtHelper.GenerateRefreshToken();

            await SaveRefreshToken(user, refreshToken);

            return new AuthTokenResponse
            {
                AccessToken = _jwtHelper.GenerateJwt(user),
                RefreshToken = refreshToken
            };
        }

        private async Task SaveRefreshToken(User user, string refreshToken)
        {
            user.RefreshToken = refreshToken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
