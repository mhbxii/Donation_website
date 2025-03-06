using Microsoft.AspNetCore.Mvc;
using dotnet9.Dtos.Account;
using dotnet9.Models;
using dotnet9.Repositories.Interfaces;
using dotnet9.Helpers;
using Microsoft.AspNetCore.Authorization;
using dotnet9.Dtos.Models;
using dotnet9.Interfaces;

namespace dotnet9.Controllers
{
    
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ControllerBase{

        private readonly IUserRepository _userRepo;
        private readonly ISetUserImageService _setUserImageService;
        public UserController(IUserRepository UserRepo, ISetUserImageService setUserImageService){
            _userRepo = UserRepo;
            _setUserImageService = setUserImageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] UserQueryObject query)
        {
            return Ok(await _userRepo.GetAllUsersWithAsync(query));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<User?>> GetUserById([FromRoute] Guid id){
            var User =  await _userRepo.GetUserByIdAsync(id);
            if(User is null)
                return NotFound();
            return Ok(User);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser([FromRoute] Guid id, [FromBody] User UserV2){

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _userRepo.UpdateUser(id, UserV2);
            
            if(!result)
                return BadRequest("Failed to update User. Ensure the ID matches.");

            return Ok(new RegisterDto{
                Email = UserV2.Email,
                UserName = UserV2.UserName,
            });
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] Guid id){
            var result = await  _userRepo.DeleteUser(id);
            
            if(result)
                return NoContent();

            return NotFound();
        }

        [HttpPatch("SetUserImage/{id}")]
        public async Task<ActionResult<UserDto>> SetUserImage([FromRoute] Guid id, [FromForm] IFormFile file)
        {
            var result = await _setUserImageService.SetUserImageAsync(id, file);
            
            if(result == null)
                return BadRequest();

            return Ok(result);
        }

        /*[HttpGet("GetTheUserId/{userName}")]
        public async Task<ActionResult<string>> GetUserId([FromRoute] string userName){
            var userId = await _userRepo.GetTheUserId(userName);
            if(userId != null)
                return Ok(userId);

            return NotFound();
        }

        [HttpGet("GetUserByName/{userName}")]
        public async Task<ActionResult<User>> GetUserByName([FromRoute] string userName){
            var user = await _userRepo.GetUserByNameAsync(userName);
            if(user != null)
                return Ok(user);

            return NotFound();
        }*/
    }

}