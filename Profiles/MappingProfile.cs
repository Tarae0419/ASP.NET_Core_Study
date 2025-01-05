using AutoMapper;
using ServerStudy.Models;
using ServerStudy.DTOs;

namespace ServerStudy.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User -> UserDto 매핑
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.ClanName, opt => opt.MapFrom(src => src.Clan.ClanName));

            // Clan -> ClanDto 매핑
            CreateMap<Clan, ClanDto>()
                .ForMember(dest => dest.MemberNicknames, opt => opt.MapFrom(src => src.Users.Select(u => u.Nickname).ToList()));
        }
    }
}