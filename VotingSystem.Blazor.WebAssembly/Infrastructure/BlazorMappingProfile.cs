using AutoMapper;
using VotingSystem.Blazor.WebAssembly.ViewModels;
using VotingSystem.Shared.Models;

namespace VotingSystem.Blazor.WebAssembly.Infrastructure
{
    public class BlazorMappingProfile : Profile
    {
        public BlazorMappingProfile()
        {
            CreateMap<PollResponseDto, PollViewModel>(MemberList.Source);
            CreateMap<PollViewModel, PollRequestDto>(MemberList.Destination);
            //CreateMap<MovieResponseDto, MovieViewModel>(MemberList.Source);
            //CreateMap<MovieViewModel, MovieRequestDto>(MemberList.Destination);

            CreateMap<LoginViewModel, LoginRequestDto>(MemberList.Source);

            CreateMap<OptionResponseDto, OptionViewModel>(MemberList.Source);
            CreateMap<OptionViewModel, OptionRequestDto>(MemberList.Destination);

            CreateMap<UserPollResponseDto, UserPollViewModel>(MemberList.Source);
            CreateMap<UserPollViewModel, UserPollRequestDto>(MemberList.Destination);


            //CreateMap<RoomResponseDto, RoomViewModel>(MemberList.Source);
            //CreateMap<RoomViewModel, RoomRequestDto>(MemberList.Destination);

            //CreateMap<ScreeningResponseDto, ScreeningViewModel>(MemberList.Source);
            //CreateMap<ScreeningViewModel, ScreeningRequestDto>(MemberList.Destination)
            //    .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Room!.Id))
            //    .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movie!.Id));

            //CreateMap<ReservationResponseDto, ReservationViewModel>(MemberList.Destination);
            //CreateMap<SeatResponseDto, SeatViewModel>(MemberList.Destination)
            //    .ForMember(dest => dest.IsSelected, opt => opt.Ignore());

            //CreateMap<SeatViewModel, SeatRequestDto>(MemberList.Destination);
        }
    }
}
