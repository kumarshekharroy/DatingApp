
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.DTOs;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _authRepository = authRepository;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            //validate request

            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();

            if (await _authRepository.UserExists(userForRegisterDTO.Username))
                return StatusCode(StatusCodes.Status400BadRequest, "Username already exists.");

            var userToBeCreated = new User { Username = userForRegisterDTO.Username };
            var createdUser = await _authRepository.Register(userToBeCreated, userForRegisterDTO.Password);

            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            User userFromRepo = await _authRepository.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
            if (userFromRepo == null)
                return StatusCode(StatusCodes.Status401Unauthorized);

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name,userFromRepo.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = cred
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDiscriptor);

            var user = _mapper.Map<UserForListDTO>(userFromRepo);
            return StatusCode(StatusCodes.Status200OK, new { token = tokenHandler.WriteToken(token), user });

        }

    }
}