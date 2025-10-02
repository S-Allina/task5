using AutoMapper;
using Users.Application.Dto;
using Users.Domain.Entity;

namespace Users.Infrastructure
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<UserRegistrationRequestDto, ApplicationUser>();
        }
    }
}
