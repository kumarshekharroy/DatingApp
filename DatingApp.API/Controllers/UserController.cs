using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _datingRepository;
        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            _mapper = mapper;
            _datingRepository = datingRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepository.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            return StatusCode(StatusCodes.Status200OK, usersToReturn);

        }
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _datingRepository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return StatusCode(StatusCodes.Status200OK, userToReturn);

        }
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateUser(long id, UserForUpdateDTO userForUpdateDTO)
        {
            if (id != long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return StatusCode(StatusCodes.Status401Unauthorized);

            var user = await _datingRepository.GetUser(id);
            _mapper.Map(userForUpdateDTO, user);
            if (await _datingRepository.SaveAll())
                return StatusCode(StatusCodes.Status204NoContent);

            throw new System.Exception($"Updating user {id} failed on save");
        }

    }
}