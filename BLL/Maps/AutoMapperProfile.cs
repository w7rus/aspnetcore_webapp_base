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
using DTO.Models.Permission;
using DTO.Models.PermissionValue;

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

        CreateMap<PermissionValueUpdate, PermissionValue>()
            .ForMember(_ => _.Id, options => options.Ignore())
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));
        
        #endregion

        #region From DOMAIN to DTO

        CreateMap<PermissionValue, PermissionValueReadResult>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));

        CreateMap<Permission, PermissionReadFSPCollectionItemResult>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.CompareMode, options => options.MapFrom(__ => __.CompareMode))
            .ForMember(_ => _.Type, options => options.MapFrom(__ => __.Type));

        CreateMap<PermissionValue, PermissionValueReadFSPCollectionItemResult>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));

        #endregion
    }

    #endregion

    #region Utilities

    bool AutoMapperAuthorizeUserPermission(ResolutionContext context, string objFieldName)
    {
        throw new NotImplementedException("Reimplement via _appDbContext.Set<AuthorizeResult>().FromSqlRaw(...)");
        
        // var userToUserGroupService = (context
        //                                   .Options
        //                                   .ServiceCtor(typeof(IUserGroupAdvancedService)) ??
        //                               throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
        //                              IUserGroupAdvancedService ??
        //                              throw new CustomException(Localize.Error.ObjectCastFailed);
        //
        // //Get AutoMapperModelAuthorizeData from the context items
        // if (!context.Items.TryGetValue(Consts.AutoMapperModelAuthorizeDataKey,
        //         out var autoMapperModelAuthorizeDataObject))
        //     throw new CustomException(Localize.Error.ValueRetrievalFailed + " " +
        //                               nameof(autoMapperModelAuthorizeDataObject));
        //
        // var autoMapperModelAuthorizeData = autoMapperModelAuthorizeDataObject as AutoMapperModelAuthorizeData ??
        //                                    throw new CustomException(Localize.Error.ObjectCastFailed);
        //
        // //Get AutoMapperModelFieldAuthorizeData for required model field
        // if (!autoMapperModelAuthorizeData.AutoMapperModelFieldAuthorizeDatas.TryGetValue(objFieldName,
        //         out var autoMapperModelFieldAuthorizeData))
        //     throw new CustomException(
        //         Localize.Error.ValueRetrievalFailed + " " + nameof(autoMapperModelFieldAuthorizeData));
        //
        // var authorizeSystemResult = true;
        //
        // if (autoMapperModelFieldAuthorizeData.PermissionValueSystemCompared != null)
        // {
        //     authorizeSystemResult = userToUserGroupService
        //         .AuthorizePermissionToPermissionValue(
        //             autoMapperModelAuthorizeData.UserComparable,
        //             autoMapperModelFieldAuthorizeData.PermissionComparable,
        //             autoMapperModelFieldAuthorizeData.PermissionValueSystemCompared
        //         )
        //         .ConfigureAwait(false).GetAwaiter().GetResult();
        // }
        //
        // bool authorizeResult;
        //
        // if (autoMapperModelFieldAuthorizeData.CustomValueCompared != null)
        // {
        //     authorizeResult = userToUserGroupService
        //         .AuthorizePermissionToCustomValue(
        //             autoMapperModelAuthorizeData.UserComparable,
        //             autoMapperModelFieldAuthorizeData.PermissionComparable,
        //             autoMapperModelFieldAuthorizeData.CustomValueCompared
        //         )
        //         .ConfigureAwait(false).GetAwaiter().GetResult();
        // }
        // else if (autoMapperModelFieldAuthorizeData.PermissionCompared != null)
        // {
        //     authorizeResult = userToUserGroupService
        //         .AuthorizePermissionToPermission(
        //             autoMapperModelAuthorizeData.UserComparable,
        //             autoMapperModelFieldAuthorizeData.PermissionComparable,
        //             autoMapperModelAuthorizeData.UserCompared,
        //             autoMapperModelFieldAuthorizeData.PermissionCompared
        //         )
        //         .ConfigureAwait(false).GetAwaiter().GetResult();
        // }
        // else
        // {
        //     throw new CustomException(Localize.Error.PermissionComparedOrCustomValueComparedRequired);
        // }
        //
        // return authorizeResult & authorizeSystemResult;
    }

    #endregion
}