using System.Net.Http.Json;
using AutoMapper;
using VotingSystem.Blazor.WebAssembly.Exception;
using VotingSystem.Blazor.WebAssembly.Infrastructure;
using VotingSystem.Blazor.WebAssembly.Services;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Shared.Models;
using System;
using System.Net.Http.Headers;

namespace VotingSystem.Blazor.WebAssembly.Services
{
    public class PollService : BaseService, IPollService
    {
        private readonly IMapper _mapper;
        private readonly IHttpRequestUtility _httpRequestUtility;
        private readonly HttpClient _http;

        public PollService(IMapper mapper, HttpClient http, IToastService toastService, IHttpRequestUtility httpRequestUtility)
            : base(toastService)
        {
            _http = http;
            _httpRequestUtility = httpRequestUtility;
            _mapper = mapper;
        }

        public async Task<List<PollViewModel>> GetPollsAsync()
        {
            var response = await _httpRequestUtility.ExecuteGetHttpRequestAsync<List<PollViewModel>>("polls");
            return response.Response;
        }

        public async Task DeletePollAsync(Guid pollId)
        {
            try
            {
                await _httpRequestUtility.ExecuteDeleteHttpRequestAsync($"polls/{pollId}");
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleHttpError(exp.Response);
            }
        }

        public async Task<PollViewModel> GetPollByIdAsync(Guid pollId)
        {
            try
            {
                var responseDto = await _httpRequestUtility.ExecuteGetHttpRequestAsync<PollResponseDto>($"polls/{pollId}");
                return _mapper.Map<PollViewModel>(responseDto.Response);
            }
            catch (HttpRequestErrorException exp)
            {
                await HandleHttpError(exp.Response);
                return new PollViewModel();
            }
        }
    
        public Task UpdatePollAsync(PollViewModel poll)
        {
            throw new NotImplementedException();
        }

        public async Task CreatePollAsync(PollViewModel poll)
        {
            PollRequestDto pollRequestDto = _mapper.Map<PollRequestDto>(poll);
            try
            {
                _ = await _httpRequestUtility.ExecutePostHttpRequestAsync<PollRequestDto, PollResponseDto>("polls", pollRequestDto);
            }
            catch (HttpRequestErrorException ex)
            {
                await HandleHttpError(ex.Response);
                throw;
            }
        }

        public Task AddOptionAsync(OptionViewModel option, Guid pollId)
        {
            throw new NotImplementedException();
        }
    }
}
