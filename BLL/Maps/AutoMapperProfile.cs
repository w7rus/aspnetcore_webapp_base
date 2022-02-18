using System;
using AutoMapper;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Exceptions;
using Common.Models;
using Domain.Entities;
using Domain.Entities.Base;
using DTO.Models.File;

namespace BLL.Maps;

public class AutoMapperProfile : Profile
{
    // // Example authorize permission by other permission value
    // var userGroup = await _userGroupService.GetByAliasAsync("Member");
    // var permission = await _permissionService.GetByAliasAsync("uint64_user_communication_private_power");
    // var permissionCompared = await _permissionService.GetByAliasAsync("uint64_user_communication_private_power_needed");
    // var entityPermissionValueCompared = await _userGroupPermissionValueService.GetByEntityIdPermissionId(userGroup.Id,
    //     permissionCompared.Id,
    //     cancellationToken);
    // var file = _mapper.Map<File>(data, opts =>
    // {
    //     opts.Items["user"] = user;
    //     opts.Items["permission"] = permission;
    //     opts.Items["entityPermissionValueCompared"] = entityPermissionValueCompared;
    // });
    
    #region Ctor

    public AutoMapperProfile()
    {
        #region From DTO to DOMAIN

        CreateMap<FileCreate, File>()
            .ForMember(_ => _.Data, options => options.Ignore())
            .ForMember(_ => _.Metadata, options => options.Ignore())
            .ForMember(_ => _.Name, options => options.Ignore())
            .ForMember(_ => _.Size, options => options.Ignore())
            .ForMember(_ => _.User, options => options.Ignore())
            .ForMember(_ => _.ContentType, options => options.Ignore())
            .ForMember(_ => _.UserId, options => options.Ignore())
            .ForMember(_ => _.AgeRating, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) => UserToUserGroupAuthorizePermissionCustom(context));
                options.MapFrom(__ => __.AgeRating);
            });

        #endregion
    }

    #endregion

    #region Utilities

    bool UserToUserGroupAuthorizePermission(ResolutionContext context)
    {
        var userToUserGroupService = (context
                                          .Options
                                          .ServiceCtor(typeof(IUserToUserGroupService)) ??
                                      throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
                                     IUserToUserGroupService ??
                                     throw new CustomException(Localize.Error.ObjectCastFailed);

        if (!context.Items.TryGetValue("user", out var userObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);
        var user = userObject as User;

        if (!context.Items.TryGetValue("permission", out var permissionObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);
        var permission = permissionObject as Permission;

        if (!context.Items.TryGetValue("entityPermissionValueCompared",
                out var entityPermissionValueComparedObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);

        dynamic entityPermissionValueCompared =
            entityPermissionValueComparedObject;
                    
        return userToUserGroupService.AuthorizePermission(user, permission, entityPermissionValueCompared)
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }
    
    bool UserToUserGroupAuthorizePermissionCustom(ResolutionContext context)
    {
        var userToUserGroupService = (context
                                          .Options
                                          .ServiceCtor(typeof(IUserToUserGroupService)) ??
                                      throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
                                     IUserToUserGroupService ??
                                     throw new CustomException(Localize.Error.ObjectCastFailed);

        if (!context.Items.TryGetValue("user", out var userObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);
        var user = userObject as User;

        if (!context.Items.TryGetValue("permission", out var permissionObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);
        var permission = permissionObject as Permission;

        if (!context.Items.TryGetValue("valueCompared",
                out var valueComparedObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed);

        return userToUserGroupService.AuthorizePermission(user, permission, valueComparedObject as byte[])
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    #endregion
}