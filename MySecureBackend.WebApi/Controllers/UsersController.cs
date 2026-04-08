using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Interface;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Services;
using System.Text.RegularExpressions;

namespace MySecureBackend.WebApi.Controllers
{
    [ApiController]

    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        private readonly PasswordService _passwordService;


        public UsersController(IUserRepository userRepository, PasswordService passwordService)

        {

            _userRepository = userRepository;

            _passwordService = passwordService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)

        {

            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))

                return BadRequest("Username and password required.");


            if (request.Password.Length < 10)

                return BadRequest("Password must be at least 10 characters.");


            // Regex checks
            if (!Regex.IsMatch(request.Password, "[a-z]"))
                return BadRequest("Password must contain at least one lowercase letter.");


            if (!Regex.IsMatch(request.Password, "[A-Z]"))

                return BadRequest("Password must contain at least one uppercase letter.");


            if (!Regex.IsMatch(request.Password, "[0-9]"))

                return BadRequest("Password must contain at least one number.");


            if (!Regex.IsMatch(request.Password, "[^a-zA-Z0-9]"))

                return BadRequest("Password must contain at least one special character.");


            var existingUser = await _userRepository.GetByUserName(request.UserName);

            if (existingUser != null)

                return BadRequest("Username already exists.");


            var user = new User
            {

                UserName = request.UserName,

                PasswordHash = _passwordService.HashPassword(request.Password)

            };


            await _userRepository.Create(user);


            return Ok(user);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)

        {

            var user = await _userRepository.GetByUserName(request.UserName);


            if (user == null)

                return Unauthorized("Invalid username or password.");


            bool validPassword = _passwordService.VerifyPassword(request.Password, user.PasswordHash);


            if (!validPassword)

                return Unauthorized("Invalid username or password.");


            return Ok(user);

        }
    }
}
