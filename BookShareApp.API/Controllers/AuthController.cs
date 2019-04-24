using System.Threading.Tasks;
using BookShareApp.API.DataAccess;
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
        public async Task<IActionResult> Register(string username, string password)
        {
            username = username.ToLower();

            if (await _repo.UserExists(username))
                return BadRequest("User name already exists");

            return StatusCode(201);
        }
    }
}