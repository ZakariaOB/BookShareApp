using System.Threading.Tasks;
using BookShareApp.API.DataAccess;
using BookShareApp.API.Dto;
using BookShareApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShareApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userToCreate)
        {
            userToCreate.UserName = userToCreate.UserName.ToLower();

            if (await _repo.UserExists(userToCreate.UserName))
            {
                return BadRequest("User name already exists");
            }

            var user = new User
            {
                UserName = userToCreate.UserName
            };

            var createdUser = await _repo.Register(user, userToCreate.Password);

            return StatusCode(201);
        }
    }
}