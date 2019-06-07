using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookShareApp.API.DataAccess;
using BookShareApp.API.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShareApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IBookShareRepository _repo;

        private readonly IMapper _mapper;

        public UsersController(IBookShareRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            IEnumerable<UserForDetailDto> usersToReturn = _mapper.Map<IEnumerable<UserForDetailDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            UserForDetailDto userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }

    }
}