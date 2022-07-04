using System;
using AutoMapper;
using Common.Exceptions;
using Common.Models;
using Domain.Entities;
using Domain.Entities.Base;
using DTO.Models.Base;
using DTO.Models.File;
using DTO.Models.Permission;
using DTO.Models.PermissionValue;
using DTO.Models.UserGroup;

namespace BLL.Maps;

public class AutoMapperProfile : Profile
{
    #region Ctor

    public AutoMapperProfile()
    {
        #region From DTO to DOMAIN

        CreateMap<FileCreateDto, File>(MemberList.None)
            .ForMember(_ => _.AgeRating, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            })
            .ForMember(_ => _.UserId, options => options.MapFrom(__ => __.UserId));
        CreateMap<FileUpdateDto, File>(MemberList.None)
            .ForMember(_ => _.AgeRating, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            });

        CreateMap<PermissionValueCreateDto, PermissionValue>(MemberList.None)
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId))
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));
        CreateMap<PermissionValueUpdateDto, PermissionValue>(MemberList.None)
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));
        
        CreateMap<UserGroupCreateDto, UserGroup>(MemberList.None)
            .ForMember(_ => _.UserId, options => options.MapFrom(__ => __.UserId))
            .ForMember(_ => _.Alias, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            })
            .ForMember(_ => _.Description, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            })
            .ForMember(_ => _.Priority, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            });
        CreateMap<UserGroupUpdateDto, UserGroup>(MemberList.None)
            .ForMember(_ => _.Alias, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            })
            .ForMember(_ => _.Description, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            })
            .ForMember(_ => _.Priority, options =>
            {
                options.Condition((objFrom, objTo, objMemberFrom, objMemberTo, context) =>
                    AutoMapperAuthorizeUserPermission(context, options.DestinationMember.Name));
            });
        CreateMap<UserGroupTransferInitDto, UserGroupTransferRequest>(MemberList.None)
            .ForMember(_ => _.UserGroupId, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.DestUserId, options => options.MapFrom(__ => __.UserId));

        #endregion

        #region From DOMAIN to DTO

        CreateMap<Permission, PermissionReadFSPCollectionItemResultDto>()
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.CompareMode, options => options.MapFrom(__ => __.CompareMode))
            .ForMember(_ => _.Type, options => options.MapFrom(__ => __.Type));
        
        CreateMap<PermissionValue, PermissionValueCreateResultDto>();
        CreateMap<PermissionValue, PermissionValueReadResultDto>()
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));
        CreateMap<PermissionValue, PermissionValueUpdateResultDto>()
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value));
        CreateMap<PermissionValue, PermissionValueReadFSPCollectionItemResultDto>()
            .ForMember(_ => _.Value, options => options.MapFrom(__ => __.Value))
            .ForMember(_ => _.PermissionId, options => options.MapFrom(__ => __.PermissionId))
            .ForMember(_ => _.EntityId, options => options.MapFrom(__ => __.EntityId));

        CreateMap<File, FileCreateResultDto>();
        CreateMap<File, FileUpdateResultDto>()
            .ForMember(_ => _.AgeRating, options => options.MapFrom(__ => __.AgeRating));

        CreateMap<UserGroup, UserGroupCreateResultDto>();
        CreateMap<UserGroup, UserGroupReadResultDto>()
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.Description, options => options.MapFrom(__ => __.Description))
            .ForMember(_ => _.IsSystem, options => options.MapFrom(__ => __.IsSystem))
            .ForMember(_ => _.Priority, options => options.MapFrom(__ => __.Priority))
            .ForMember(_ => _.UserId, options => options.MapFrom(__ => __.UserId));
        CreateMap<UserGroup, UserGroupUpdateResultDto>()
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.Description, options => options.MapFrom(__ => __.Description))
            .ForMember(_ => _.IsSystem, options => options.MapFrom(__ => __.IsSystem))
            .ForMember(_ => _.Priority, options => options.MapFrom(__ => __.Priority))
            .ForMember(_ => _.UserId, options => options.MapFrom(__ => __.UserId));
        CreateMap<UserGroup, UserGroupReadDtoReadFSPCollectionItemResultDto>()
            .ForMember(_ => _.Alias, options => options.MapFrom(__ => __.Alias))
            .ForMember(_ => _.Description, options => options.MapFrom(__ => __.Description))
            .ForMember(_ => _.IsSystem, options => options.MapFrom(__ => __.IsSystem))
            .ForMember(_ => _.Priority, options => options.MapFrom(__ => __.Priority))
            .ForMember(_ => _.UserId, options => options.MapFrom(__ => __.UserId));

        CreateMap<UserGroupTransferRequest, UserGroupTransferInitResultDto>();

        CreateMap<UserGroupInviteRequest, UserGroupInitInviteUserResultDto>();
        
        CreateMap<EntityBase<Guid>, IEntityBaseResultDto<Guid>>()
            .ForMember(_ => _.Id, options => options.MapFrom(__ => __.Id))
            .ForMember(_ => _.CreatedAt, options => options.MapFrom(__ => __.CreatedAt))
            .ForMember(_ => _.UpdatedAt, options => options.MapFrom(__ => __.UpdatedAt));

        #endregion
    }

    #endregion

    #region Utilities

    private bool AutoMapperAuthorizeUserPermission(ResolutionContext context, string objFieldName)
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