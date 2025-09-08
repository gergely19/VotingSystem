using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Exceptions;

namespace VotingSystem.DataAccess.Services
{
    public class UserPollService : IUserPollService
    {
        private readonly VotingDbContext _context;

        public UserPollService(VotingDbContext context)
        {
            _context = context;
        }

        public async Task<UserPoll> GetByUserAndPollAsync(Guid userId, Guid pollId)
        {
            var userPoll = await _context.UserPolls
                .FirstOrDefaultAsync(up => up.UserId == userId.ToString() && up.PollId == pollId);

            if (userPoll == null)
            {
                throw new EntityNotFoundException($"UserPoll with UserId '{userId}' and PollId '{pollId}' was not found.");
            }

            return userPoll;
        }

        public async Task AddAsync(UserPoll userPoll)
        {
            try
            {
                await _context.UserPolls.AddAsync(userPoll);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Failed to create user poll.", ex);
            }
        }

        public async Task UpdateAsync(UserPoll userPoll)
        {
            var existingUserPoll = await GetByUserAndPollAsync(Guid.Parse(userPoll.UserId), userPoll.PollId);

            if (existingUserPoll == null)
                throw new EntityNotFoundException(nameof(UserPoll));

            try
            {
                _context.Entry(existingUserPoll).State = EntityState.Detached;
                _context.UserPolls.Update(userPoll);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Failed to update user poll.", ex);
            }
        }

        public async Task<IReadOnlyCollection<UserPoll>> GetAllByPollIdAsync(Guid pollId)
        {
            return await _context.UserPolls
                .Where(up => up.PollId == pollId)
                .ToListAsync();
        }
    }
}
