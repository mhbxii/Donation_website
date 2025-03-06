using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using dotnet9.Dtos.Account;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;
using dotnet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace dotnet9.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IImageUploadService _imageUploadService;

        public AuthService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           IConfiguration configuration,
                           ITokenService tokenService,
                           IEmailService emailService,
                           IImageUploadService imageUploadService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
            _tokenService = tokenService;
            _emailService = emailService;
            _imageUploadService = imageUploadService;
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Unauthorized");

            return new UserDto { Id = user.Id, UserName = user.UserName, Email = user.Email };
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var imageUrl = await _imageUploadService.UploadImageAsync(dto.ImageFile!);
            var user = new User { 
                UserName = dto.UserName, 
                Email = dto.Email, 
                Origin = dto.Origin,  
                PhoneNumber = dto.PhoneNumber,
                ImageUrl = imageUrl
            };
            var result = await _userManager.CreateAsync(user, dto.Password!);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
                throw new Exception(string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            return await GenerateAuthResultAsync(user);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid Email");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, false);
            if (!result.Succeeded)
                throw new Exception("Invalid user or password");

            return await GenerateAuthResultAsync(user);
        }
        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token / try logging in again!");

            return await GenerateAuthResultAsync(user);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = DateTime.UnixEpoch;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        public async Task RequestResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Oops, something went wrong.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(resetToken);
            var resetUrl = $"{_config["ResetPasswordFrontendUrl"]}?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";
            var subject = "Password Reset Request";
            var htmlContent = $"Please reset your password by clicking <a href='{resetUrl}'>here</a>.";
            await _emailService.SendEmailAsync(user.Email!, subject, htmlContent);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid request.");

            var resetResult = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!resetResult.Succeeded)
                throw new Exception(string.Join(", ", resetResult.Errors.Select(e => e.Description)));
        }

        // Helper that generates tokens, updates the user, and returns an AuthResultDto.
        private async Task<AuthResultDto> GenerateAuthResultAsync(User user)
        {
            var accessToken = _tokenService.CreateToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResultDto
            {
                User = new UserDto {Id = user.Id, UserName = user.UserName, Email = user.Email, Origin = user.Origin, ImageUrl = user.ImageUrl, PhoneNumber = user.PhoneNumber },
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
