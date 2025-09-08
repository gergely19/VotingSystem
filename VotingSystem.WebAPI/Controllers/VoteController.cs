using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Exceptions;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;

namespace VotingSystem.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing votes for polls.
    /// </summary>
    [Route("/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVoteService _voteService;
        private readonly IOptionService _optionService;
        private readonly IUserService _userService;
        private readonly IUserPollService _userPollService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoteController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper for DTO conversions.</param>
        /// <param name="voteService">The service for vote operations.</param>
        /// <param name="optionService">The service for poll option operations.</param>
        /// <param name="userService">The service for user-poll operations.</param>
        /// <param name="userPollService"></param>
        public VoteController(IMapper mapper, IVoteService voteService, IOptionService optionService, IUserService userService, IUserPollService userPollService)
        {
            _mapper = mapper;
            _voteService = voteService;
            _optionService = optionService;
            _userService = userService;
            _userPollService = userPollService;
        }

        /// <summary>
        /// Casts a vote for the specified option.
        /// </summary>
        /// <param name="voteRequestDto">The vote request data transfer object containing the selected option ID.</param>
        /// <response code="200">Vote recorded successfully.</response>
        /// <response code="400">The request is invalid or user ID is missing.</response>
        /// <response code="403">The user is not authorized to vote.</response>
        /// <response code="404">The specified option was not found.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VoteAsync([FromBody] VoteRequestDto voteRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = _userService.GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var vote = _mapper.Map<Vote>(voteRequestDto);
                var option = await _optionService.GetByIdAsync(vote.OptionId);

                var userpoll = new UserPoll
                {
                    UserId = userId.ToString(),
                    PollId = option.PollId,
                    HasVoted = true
                };

                await _userPollService.AddAsync(userpoll);
                await _voteService.AddAsync(vote);

                return Ok(new { Message = "Vote recorded successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }



    }
}
