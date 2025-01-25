using System.Linq; // 추가: LINQ 사용을 위해 필요
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
                .ForMember(
                    dest => dest.ClanName, 
                    opt => opt.MapFrom(src => src.Clan != null ? src.Clan.ClanName : null) // Clan이 null일 경우 처리
                );

            // Clan -> ClanDto 매핑
            CreateMap<Clan, ClanDto>()
                .ForMember(
                    dest => dest.MemberNicknames, 
                    opt => opt.MapFrom(src => src.Users != null ? src.Users.Select(u => u.Nickname).ToList() : new List<string>()) // Users가 null일 경우 처리
                );
        }
    }
}