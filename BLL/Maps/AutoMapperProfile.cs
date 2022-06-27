using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using Domain.Entities;
using Domain.Entities.Base;
using DTO.Models.File;
using DTO.Models.Permission;
using DTO.Models.PermissionValue;
using Microsoft.EntityFrameworkCore;

namespace BLL.Maps;

public class AutoMapperProfile : Profile
{
    #region Ctor

    public AutoMapperProfile()
    {
        #region From DTO to DOMAIN

        CreateMap<FileCreateDto, File>()
            .ForMember(_ => _.Stream, options => options.Ignore())
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

        CreateMap<FileUpdateDto, File>()
            .ForMember(_ => _.Stream, options => options.Ignore())
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

        CreateMap<PermissionValueUpdateDto, PermissionValue>()
            .ForMember(_ => _.Id, options => options.Ignore())
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));

        #endregion

        #region From DOMAIN to DTO

        CreateMap<PermissionValue, PermissionValueReadResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));

        CreateMap<PermissionValue, PermissionValueUpdateResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));

        CreateMap<Permission, PermissionReadFSPCollectionItemResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.CompareMode, options => options.MapFrom(__ => __.CompareMode))
            .ForMember(_ => _.Type, options => options.MapFrom(__ => __.Type));

        CreateMap<PermissionValue, PermissionValueReadFSPCollectionItemResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));
        
        CreateMap<File, FileCreateResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.AgeRating, options => options.MapFrom(__ => __.AgeRating));

        CreateMap<File, FileUpdateResultDto>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.AgeRating, options => options.MapFrom(__ => __.AgeRating));

        #endregion
    }

    #endregion

    #region Utilities

    bool AutoMapperAuthorizeUserPermission(ResolutionContext context, string objFieldName)
    {
        //Get AutoMapperModelAuthorizeData from the context items
        if (!context.Items.TryGetValue(Consts.AutoMapperModelAuthorizeDataKey,
                out var autoMapperModelAuthorizeDataObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " +
                                      nameof(autoMapperModelAuthorizeDataObject));

        var autoMapperModelAuthorizeData = autoMapperModelAuthorizeDataObject as AutoMapperModelAuthorizeData ??
                                           throw new CustomException(Localize.Error.ObjectCastFailed);

        //Get AutoMapperModelFieldAuthorizeData for required model field
        if (!autoMapperModelAuthorizeData.FieldAuthorizeResultDictionary.TryGetValue(objFieldName,
                out var authorizeResult))
            throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(authorizeResult));

        return authorizeResult;
    }

    #endregion
}