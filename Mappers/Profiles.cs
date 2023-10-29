using AutoMapper;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Accesses;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Models.UserPrivileges;
using Dorbit.Identity.Models.Users;
using UserDto = Dorbit.Models.Users.UserDto;

namespace Dorbit.Identity.Mappers;

public class Profiles : Profile
{
    public void CreateMapTwoWay<T, TR>()
    {
        CreateMap<T, TR>();
        CreateMap<TR, T>();
    }
    
    public Profiles()
    {
        CreateMapTwoWay<Access, AccessDto>();
        
        CreateMapTwoWay<Token, TokenDto>();
        
        CreateMapTwoWay<Privilege, PrivilegeDto>();
        
        CreateMapTwoWay<User, UserDto>();
        CreateMap<UserAddRequest, User>();
        CreateMap<UserEditRequest, User>();
    }
}