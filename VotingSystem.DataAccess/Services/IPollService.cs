using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Voting.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface IPollService
    {
        Task<IReadOnlyCollection<Poll>> GetActivesAsync();
        Task<IReadOnlyCollection<Poll>> GetClosedAsync();
        Task<IReadOnlyCollection<Poll>> GetPollsByUserIdAsync(Guid userId);
        Task<IReadOnlyCollection<Poll>> GetLoggedInPollsAsync();
        Task<Poll> GetByIdAsync(Guid id);
        Task AddAsync(Poll poll);
        Task UpdateAsync(Poll poll);
        Task<bool> ExistsAsync(Expression<Func<Poll, bool>> predicate);

    }
}
