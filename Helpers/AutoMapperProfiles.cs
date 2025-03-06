using AutoMapper;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Extensions;

namespace DatingAppAPI.Helpers;

public class AutoMapperProfiles : Profile
{
public AutoMapperProfiles()
 {
    CreateMap<AppUser, MemberDTO>()
    .ForMember(dest => dest.Age,
     opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
    .ForMember(dest => dest.PhotoUrl,
     opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
   
    CreateMap<Photo, PhotoDTO>();
 }
}
