using System;
using System.Collections.Generic;
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
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, objMemberTo.GetType().Name));
                // options.MapFrom(__ => __.AgeRating);
            });
        
        CreateMap<FileUpdate, File>()
            .ForMember(_ => _.Data, options => options.Ignore())
            .ForMember(_ => _.Metadata, options => options.Ignore())
            .ForMember(_ => _.Name, options => options.Ignore())
            .ForMember(_ => _.Size, options => options.Ignore())
            .ForMember(_ => _.User, options => options.Ignore())
            .ForMember(_ => _.ContentType, options => options.Ignore())
            .ForMember(_ => _.UserId, options => options.Ignore())
            .ForMember(_ => _.AgeRating, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, objMemberTo.GetType().Name));
                // options.MapFrom(__ => __.AgeRating);
            });

        #endregion
    }

    #endregion

    #region Utilities

    bool AutoMapperAuthorizeUserPermission(ResolutionContext context, string objFieldName)
    {
        var userToUserGroupService = (context
                                          .Options
                                          .ServiceCtor(typeof(IUserToUserGroupAdvancedService)) ??
                                      throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
                                     IUserToUserGroupAdvancedService ??
                                     throw new CustomException(Localize.Error.ObjectCastFailed);

        //Get AutoMapperModelAuthorizeData from the context items
        if (!context.Items.TryGetValue(Consts.AutoMapperModelAuthorizeDataKey,
                out var autoMapperModelAuthorizeDataObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " +
                                      nameof(autoMapperModelAuthorizeDataObject));

        var autoMapperModelAuthorizeData = autoMapperModelAuthorizeDataObject as AutoMapperModelAuthorizeData ??
                                           throw new CustomException(Localize.Error.ObjectCastFailed);

        //Get AutoMapperModelFieldAuthorizeData for required model field
        if (!autoMapperModelAuthorizeData.AutoMapperModelFieldAuthorizeDatas.TryGetValue(objFieldName,
                out var autoMapperModelFieldAuthorizeData))
            throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(autoMapperModelFieldAuthorizeData));

        var authorizeSystemResult = true;
        
        if (autoMapperModelFieldAuthorizeData.PermissionValueSystemCompared != null)
        {
            authorizeSystemResult = userToUserGroupService
                .AuthorizeUserPermissionToAnyPermissionValue(
                    autoMapperModelAuthorizeData.UserComparable,
                    autoMapperModelFieldAuthorizeData.PermissionComparable,
                    autoMapperModelFieldAuthorizeData.PermissionValueSystemCompared
                )
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        bool authorizeResult;

        if (autoMapperModelFieldAuthorizeData.CustomValueCompared != null)
        {
            authorizeResult = userToUserGroupService
                .AuthorizeUserPermissionToCustomValue(
                    autoMapperModelAuthorizeData.UserComparable,
                    autoMapperModelFieldAuthorizeData.PermissionComparable,
                    autoMapperModelFieldAuthorizeData.CustomValueCompared
                )
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else if (autoMapperModelFieldAuthorizeData.PermissionCompared != null)
        {
            authorizeResult = userToUserGroupService
                .AuthorizeUserPermissionToUserPermission(
                    autoMapperModelAuthorizeData.UserComparable,
                    autoMapperModelFieldAuthorizeData.PermissionComparable,
                    autoMapperModelAuthorizeData.UserCompared,
                    autoMapperModelFieldAuthorizeData.PermissionCompared
                )
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
        else
        {
            throw new CustomException(Localize.Error.PermissionComparedOrCustomValueComparedRequired);
        }
        
        return authorizeResult & authorizeSystemResult;
    }

    #endregion
}