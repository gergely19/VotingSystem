using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voting.DataAccess.Models;
using VotingSystem.DataAccess.Exceptions;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public class OptionService : IOptionService
    {
        private readonly VotingDbContext _context;

        public OptionService(VotingDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Option>> GetAllByPollIdAsync(Guid pollId)
        {
            return await _context.Options
                .Where(o => o.PollId == pollId && !o.DeletedAt.HasValue)
                .ToListAsync();
        }

        public async Task<Option> GetByIdAsync(Guid id)
        {
            var option = await _context.Options
                .FirstOrDefaultAsync(o => o.Id == id && !o.DeletedAt.HasValue);

            if (option == null)
                throw new EntityNotFoundException(nameof(Option));

            return option;
        }

        public async Task DeleteAsync(Guid id)
        {
            var option = await GetByIdAsync(id);

            option.DeletedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Failed to delete option.", ex);
            }
        }

        public async Task AddAsync(Option option)
        {
            await CheckIfOptionTextExistsAsync(option);

            try
            {
                await _context.Options.AddAsync(option);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Failed to create option.", ex);
            }
        }

        public async Task UpdateAsync(Option option)
        {
            var existingOption = await GetByIdAsync(option.Id);

            if (existingOption == null)
                throw new EntityNotFoundException(nameof(Option));

            await CheckIfOptionTextExistsAsync(option);

            try
            {
                _context.Entry(existingOption).State = EntityState.Detached;
                _context.Options.Update(option);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new SaveFailedException("Failed to update option.", ex);
            }
        }

        private async Task CheckIfOptionTextExistsAsync(Option option)
        {
            if (await _context.Options.AnyAsync(o => o.Id != option.Id
                                                    && o.PollId == option.PollId
                                                    && !o.DeletedAt.HasValue
                                                    && o.Text.ToLower() == option.Text.ToLower()))
            {
                throw new InvalidDataException("Option with the same text already exists.");
            }
        }
    }
}
