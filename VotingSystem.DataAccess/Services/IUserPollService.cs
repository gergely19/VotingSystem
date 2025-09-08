using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface IUserPollService
    {
        Task<UserPoll> GetByUserAndPollAsync(Guid userId, Guid pollId);
        Task AddAsync(UserPoll userPoll);
        Task UpdateAsync(UserPoll userPoll);
        Task<IReadOnlyCollection<UserPoll>> GetAllByPollIdAsync(Guid pollId);
    }
}
