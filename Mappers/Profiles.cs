using AutoMapper;
using Dorbit.Framework.Models.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Accesses;
using Dorbit.Identity.Models.Privileges;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Models.Users;

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
        CreateMap<Access, AccessDto>();
        CreateMap<AccessAddDto, Access>();
        CreateMap<AccessEditDto, Access>();

        CreateMapTwoWay<Token, TokenDto>();

        CreateMapTwoWay<Privilege, PrivilegeDto>();
        CreateMapTwoWay<PrivilegeSaveRequest, Privilege>();

        CreateMapTwoWay<User, UserIdentityDto>();
        CreateMap<UserDto, UserIdentityDto>();
        CreateMap<UserAddRequest, User>();
        CreateMap<UserEditRequest, User>();
    }
}