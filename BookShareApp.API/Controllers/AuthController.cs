using System;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading.Tasks;
using BookShareApp.API.DataAccess;
using BookShareApp.API.Dto;
using BookShareApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using BookShareApp.API.Framework;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace BookShareApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public IMapper _mapper { get; }

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            this._mapper = mapper;
            _repo = repo;
            _config = config;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForRegisterDto userDto)
        {
            var userFromRepo = await _repo.Login(userDto.UserName, userDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var appSettingsConfig = _config.GetSection(Constants.AppSettingsToken).Value;
            var tokenResult = Helepr.GenerateJwtToken(userFromRepo.Id.ToString(), userFromRepo.UserName, appSettingsConfig);
            var user = _mapper.Map<UserForListDto>(userFromRepo);

            var returnedToken = new
            {
                token = tokenResult,
                user = user
            };

            return Ok(returnedToken);
        }
    }
}