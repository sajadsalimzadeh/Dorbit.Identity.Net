using AutoMapper;
using Dorbit.Framework.Contracts.Users;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;

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

        CreateMapTwoWay<UserPrivilege, PrivilegeDto>();
        CreateMapTwoWay<PrivilegeSaveRequest, UserPrivilege>();

        CreateMap<User, UserDto>()
            .ForMember(x => x.HasPassword, o => o.MapFrom(x => !string.IsNullOrEmpty(x.PasswordHash)));
        CreateMap<UserDto, User>();
        CreateMap<User, BaseUserDto>();
        CreateMap<BaseUserDto, UserDto>();
        CreateMap<UserAddRequest, User>()
            .ForMember(x => x.Cellphone, o => o.MapFrom(x => string.IsNullOrEmpty(x.Cellphone) ? null : x.Cellphone))
            .ForMember(x => x.Email, o => o.MapFrom(x => string.IsNullOrEmpty(x.Email) ? null : x.Email));
        CreateMap<UserEditRequest, User>()
            .ForMember(x => x.Cellphone, o => o.MapFrom(x => string.IsNullOrEmpty(x.Cellphone) ? null : x.Cellphone))
            .ForMember(x => x.Email, o => o.MapFrom(x => string.IsNullOrEmpty(x.Email) ? null : x.Email));
    }
}