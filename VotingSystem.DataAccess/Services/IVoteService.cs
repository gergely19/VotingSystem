using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface IVoteService
    {
        Task AddAsync(Vote vote);
        Task<IReadOnlyCollection<Vote>> GetVotesByOptionAsync(Guid optionId);
    }
}