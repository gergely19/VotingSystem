using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebAPI.Controllers
{
    /// <summary>
    /// UsersController
    /// </summary>
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _usersService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="usersService"></param>
        /// <param name="userManager"></param>
        public UsersController(IMapper mapper, IUserService usersService, UserManager<User> userManager)
        {
            _mapper = mapper;
            _usersService = usersService;
            _userManager = userManager;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userRequestDto"></param>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="409">Conflict</response>
        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestDto userRequestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRequestDto.Email);
            if (existingUser != null)
            {
                return Conflict($"User with email '{userRequestDto.Email}' already exists.");
            }
            var user = _mapper.Map<User>(userRequestDto);

            try
            {
                await _usersService.AddUserAsync(user, userRequestDto.Password);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var userResponseDto = _mapper.Map<UserResponseDto>(user);

            return StatusCode(StatusCodes.Status201Created, userResponseDto);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">User</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(UserResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute][Required] string id)
        {
            try
            {
                var user = await _usersService.GetUserByIdAsync(id);
                var userDto = _mapper.Map<UserResponseDto>(user);
                return Ok(userDto);
            }
            catch (AccessViolationException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="loginRequestDto"></param>
        /// <response code="200">Login was successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var (authToken, refreshToken, userId) = await _usersService.LoginAsync(loginRequestDto.Email, loginRequestDto.Password);

                var loginResponseDto = new LoginResponseDto
                {
                    UserId = userId,
                    AuthToken = authToken,
                    RefreshToken = refreshToken,
                };

                return Ok(loginResponseDto);
            }
            catch (AccessViolationException)
            {
                return StatusCode(429);

            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }



        /// <summary>
        /// Logout
        /// </summary>
        /// <response code="200">Logout was successful</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("logout")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent, type: typeof(void))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _usersService.LogoutAsync();

            return NoContent();
        }

        /// <summary>
        /// Redeem refresh token
        /// </summary>
        /// <response code="200">Login was successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="403">Forbidden</response>
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RedeemRefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest();

            try
            {
                var (authToken, newRefreshToken, userId) = await _usersService.RedeemRefreshTokenAsync(refreshToken);
                if (string.IsNullOrEmpty(newRefreshToken))
                    return Forbid();

                var loginResponseDto = new LoginResponseDto
                {
                    UserId = userId,
                    AuthToken = authToken,
                    RefreshToken = newRefreshToken,
                };

                return Ok(loginResponseDto);
            }
            catch
            {
                return Forbid();
            }
        }


    }
}
