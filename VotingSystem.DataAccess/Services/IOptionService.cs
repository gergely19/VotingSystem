using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface IOptionService
    {
        Task<IReadOnlyCollection<Option>> GetAllByPollIdAsync(Guid pollId);
        Task<Option> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task AddAsync(Option option);
        Task UpdateAsync(Option option);
    }
}
