using AutoMapper;
using VotingSystem.Shared.Models;
using VotingSystem.DataAccess.Models;
using Voting.DataAccess.Models;


namespace VotingSystem.WebAPI.Infrasturture
{
    /// <summary>  
    /// Defines the mapping configuration for AutoMapper.  
    /// </summary>  
    public class MappingProfile : Profile
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.  
        /// Configures mappings between DTOs and domain models.  
        /// </summary>  
        public MappingProfile()
        {
            CreateMap<UserRequestDto, User>(MemberList.Source)
                .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<User, UserResponseDto>(MemberList.Destination);

            CreateMap<OptionRequestDto, Option>(MemberList.Source);
            CreateMap<Option, OptionResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => src.Votes != null ? src.Votes.Count : 0));

            CreateMap<UserPollRequestDto, UserPoll>(MemberList.Source);
            CreateMap<UserPoll, UserPollResponseDto>(MemberList.Destination);

            CreateMap<VoteRequestDto, Vote>(MemberList.Source);
            CreateMap<Vote, VoteResponseDto>(MemberList.Destination);

            CreateMap<PollRequestDto, Poll>(MemberList.Source);
            CreateMap<Poll, PollResponseDto>(MemberList.Destination);
        }
    }
}
