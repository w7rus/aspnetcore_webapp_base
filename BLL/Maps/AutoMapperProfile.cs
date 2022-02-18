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
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) => UserToUserGroupAuthorizePermission(context));
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

    #endregion
}