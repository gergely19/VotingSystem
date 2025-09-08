using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Exceptions;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public class PollService : IPollService
    {
        private readonly VotingDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<PollService> _logger;

        public PollService(VotingDbContext context, IUserService userService, ILogger<PollService> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<Poll>> GetActivesAsync()
        {
            return await _context.Polls
                .Where(p => p.DeletedAt == null && p.EndDate >= DateTime.Now && p.StartDate <= DateTime.Now)
                .ToListAsync();
        }
        public async Task<IReadOnlyCollection<Poll>> GetPollsByUserIdAsync(Guid userId)
        {
            return await _context.Polls
                .Include(p => p.Options)
                .Where(p => !p.DeletedAt.HasValue && p.CreatedById == userId.ToString())
                .ToListAsync();
        }

        // Egy poll lekérése azonosítóval (kivételt dob, ha nincs ilyen)
        public async Task<Poll> GetByIdAsync(Guid id)
        {
            var poll = await _context.Polls
                .Include(p => p.Options)
                .Include(p => p.UserPolls)
                .FirstOrDefaultAsync(p => p.Id == id && !p.DeletedAt.HasValue);

            if (poll == null)
                throw new EntityNotFoundException(nameof(Poll));

            return poll;
        }

        /*
        // Poll "törlése" (puha törlés: DeletedAt dátumot állít)
        public async Task DeleteAsync(Guid id)
        {
            var poll = await GetByIdAsync(id);

            poll.DeletedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Hiba a szavazás törlése közben.", ex);
            }
        }*/

        // Új szavazás hozzáadása (ellenőrzés a duplikációra)
        public async Task AddAsync(Poll poll)
        {
            await CheckIfQuestionExistsAsync(poll);
            poll.Id = Guid.NewGuid();
            poll.CreatedAt = DateTime.UtcNow;
            if (poll.Options != null)
            {
                foreach (var option in poll.Options)
                {
                    option.Id = Guid.NewGuid();
                    option.PollId = poll.Id;
                }
            }
            
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
            {
                throw new Exception("Felhasználó nem található.");
            }
            poll.CreatedBy = await _userService.GetCurrentUserAsync();
            poll.CreatedById = userId;
            try
            {
                await _context.Polls.AddAsync(poll);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Hiba a szavazás mentésekor.", ex);
            }
        }

        // Szavazás módosítása (option duplikáció ellenőrzése, CreatedAt megtartása)
        public async Task UpdateAsync(Poll poll)
        {
            var existingPoll = await GetByIdAsync(poll.Id);

            foreach (var option in poll.Options!)
            {
                await CheckIfOptionTextExistsAsync(option, poll.Id);
            }

            poll.CreatedAt = existingPoll.CreatedAt; // Létrehozási dátum ne változzon

            try
            {
                _context.Entry(existingPoll).State = EntityState.Detached;
                _context.Polls.Update(poll);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Hiba a szavazás módosításakor.", ex);
            }
        }


        // Ellenőrzi, létezik-e már ilyen Question (nem érzékeny a kis/nagybetűkre)
        private async Task CheckIfQuestionExistsAsync(Poll poll)
        {
            if (await _context.Polls
                .AnyAsync(p => p.Id != poll.Id
                               && !p.DeletedAt.HasValue
                               && p.Question.ToLower() == poll.Question.ToLower()))
                throw new InvalidDataException("Már létezik ilyen kérdéssel szavazás.");
        }

        // Ellenőrzi, van-e már ilyen Option szöveg adott pollban (nem érzékeny a kis/nagybetűkre)
        private async Task CheckIfOptionTextExistsAsync(Option option, Guid pollId)
        {
            if (await _context.Options
                .AnyAsync(o => o.Id != option.Id
                               && o.PollId == pollId
                               && !o.DeletedAt.HasValue
                               && o.Text.ToLower() == option.Text.ToLower()))
                throw new InvalidDataException("Ebben a szavazásban már létezik ilyen opció.");
        }

        public async Task<IReadOnlyCollection<Poll>> GetLoggedInPollsAsync()
        {
            return await _context.Polls
            .Where(p => p.DeletedAt == null && p.CreatedById == _userService.GetCurrentUserId())
            .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Poll>> GetClosedAsync()
        {
            return await _context.Polls
                .Where(p => p.DeletedAt == null && p.EndDate < DateTime.Now)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Poll, bool>> predicate)
        {
            return await _context.Polls.AnyAsync(predicate);
        }

    }
}