using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public class VoteService : IVoteService
    {
        private readonly VotingDbContext _context;

        public VoteService(VotingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Vote>> GetVotesByOptionAsync(Guid optionId)
        {
            return await _context.Votes
                .Where(v => v.OptionId == optionId && !v.DeletedAt.HasValue)
                .ToListAsync();
        }
    }
}
