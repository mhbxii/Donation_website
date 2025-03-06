using dotnet9.Dtos.Account;
using dotnet9.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet9.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userDto = await _authService.GetCurrentUserAsync(User);
            return Ok(userDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                var authResult = await _authService.RegisterAsync(registerDto);
                SetTokenCookies(authResult.AccessToken, authResult.RefreshToken);
                return Ok(authResult.User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var authResult = await _authService.LoginAsync(loginDto);
                SetTokenCookies(authResult.AccessToken, authResult.RefreshToken);
                return Ok(authResult.User);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("NewAccessToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            try
            {
                var authResult = await _authService.RefreshTokenAsync(refreshToken!);
                SetTokenCookies(authResult.AccessToken, authResult.RefreshToken);
                return Ok(authResult.User);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            await _authService.LogoutAsync(refreshToken!);
            DeleteTokenCookies();
            return Ok(new { message = "Logged out" });
        }

        [HttpPost("RequestResetPassword")]
        public async Task<IActionResult> RequestResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            try
            {
                await _authService.RequestResetPasswordAsync(dto);
                return Ok(new { message = "Password reset link sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto);
                return Ok(new { message = "Password has been reset." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Private Helper Methods for Cookie Handling

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };
            Response.Cookies.Append("access_token", accessToken, accessCookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
        }

        private void DeleteTokenCookies()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
        }

        #endregion
    }
}
