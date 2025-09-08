using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Exceptions;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebAPI.Controllers
{
    /// <summary>  
    /// Controller for managing polls.  
    /// </summary>  
    [Route("/polls")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPollService _pollService;
        private readonly IUserService _userService;

        /// <summary>  
        /// Initializes a new instance of the <see cref="PollsController"/> class.  
        /// </summary>  
        /// <param name="mapper">The mapper instance.</param>  
        /// <param name="pollService">The poll service instance.</param>
        /// <param name="userService"></param>  
        public PollsController(IMapper mapper, IPollService pollService, IUserService userService)
        {
            _mapper = mapper;
            _pollService = pollService;
            _userService = userService;
        }

        /// <summary>  
        /// Retrieves loggeed in user polls.  
        /// </summary>  
        /// <returns>A list of polls of logged in user.</returns>  
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<PollResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPolls()
        {
            /*var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var polls = await _pollService.GetPollsByUserIdAsync(Guid.Parse(userId));*/
            var polls = await _pollService.GetLoggedInPollsAsync();
            var pollsDto = _mapper.Map<IReadOnlyCollection<PollResponseDto>>(polls);
            return Ok(pollsDto);
        }

        /// <summary>  
        /// Retrieves active polls.  
        /// </summary>  
        /// <returns>A list of active polls.</returns>  
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("actives")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<PollResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActivesPolls()
        {
            var polls = await _pollService.GetActivesAsync();
            var pollsDto = _mapper.Map<IReadOnlyCollection<PollResponseDto>>(polls);
            return Ok(pollsDto);
        }

        /// <summary>  
        /// Retrieves closed polls.
        /// </summary>  
        /// <returns>A list of polls.</returns> 
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("closed")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<PollResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetClosedPolls()
        {
            var polls = await _pollService.GetClosedAsync();
            var pollsDto = _mapper.Map<IReadOnlyCollection<PollResponseDto>>(polls);
            return Ok(pollsDto);
        }

        /// <summary>
        /// Get a poll by ID
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">The requested poll</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(PollResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPollById([FromRoute] Guid id)
        {
            try
            {
                var poll = await _pollService.GetByIdAsync(id);
                var pollResponseDto = _mapper.Map<PollResponseDto>(poll);

                return Ok(pollResponseDto);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add a new poll
        /// </summary>
        /// <param name="pollRequestDto"></param>
        /// <response code="201">Poll created successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Conflict</response>    
        [HttpPost]
        [Authorize]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(PollResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePoll([FromBody] PollRequestDto pollRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (pollRequestDto.StartDate >= pollRequestDto.EndDate || pollRequestDto.StartDate <= DateTime.UtcNow)
                return Conflict();

            var poll = _mapper.Map<Poll>(pollRequestDto);
            poll.CreatedAt = DateTime.UtcNow;
            poll.CreatedById = userId;

            try
            {
                await _pollService.AddAsync(poll);
            }
            catch (Exception ex)
            {
                return Conflict($"Could not create poll: {ex.Message}");
            }

            var pollResponseDto = _mapper.Map<PollResponseDto>(poll);
            return CreatedAtAction(nameof(GetPollById), new { id = pollResponseDto.Id }, pollResponseDto);
        }



    }
}
