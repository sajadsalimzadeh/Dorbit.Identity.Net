using AutoMapper;
using Dorbit.Framework.Contracts.Users;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Databases.Entities;

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

        CreateMapTwoWay<User, UserDto>();
        CreateMap<User, BaseUserDto>();
        CreateMap<BaseUserDto, UserDto>();
        CreateMap<UserAddRequest, User>();
        CreateMap<UserEditRequest, User>();
    }
}