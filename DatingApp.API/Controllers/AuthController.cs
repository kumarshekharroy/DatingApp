
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.DTOs;

namespace DatingApp.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;

        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            //validate request

            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();

            if (await _authRepository.UserExists(userForRegisterDTO.Username))
                return StatusCode(StatusCodes.Status400BadRequest,"Username already exists.");

            var userToBeCreated = new User { Username = userForRegisterDTO.Username };
            var createdUser = await _authRepository.Register(userToBeCreated, userForRegisterDTO.Password);

            return StatusCode(StatusCodes.Status201Created);
        }


    }
}