using VotingSystem.Blazor.WebAssembly.ViewModels;

namespace VotingSystem.Blazor.WebAssembly.Services
{
    public interface IPollService
    {
        public Task<List<PollViewModel>> GetPollsAsync();
        public Task<PollViewModel> GetPollByIdAsync(Guid pollId);
        public Task UpdatePollAsync(PollViewModel poll);
        public Task CreatePollAsync(PollViewModel poll);
        public Task DeletePollAsync(Guid pollId);
        public Task AddOptionAsync(OptionViewModel option, Guid pollId);
    }
}
