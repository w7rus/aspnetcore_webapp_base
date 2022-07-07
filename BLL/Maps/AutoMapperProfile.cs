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
using DTO.Models.UserGroupActions;

namespace BLL.Maps;

public class AutoMapperProfile : Profile
{
    #region Ctor

    public AutoMapperProfile()
    {
        #region From DTO to DOMAIN

        CreateMap<FileCreateDto, File>(MemberList.None)
            .ForMember(_ => _.AgeRating, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            })
            .ForMember(_ => _.UserId, opt => opt.MapFrom(__ => __.TargetUserId));
        CreateMap<FileUpdateDto, File>(MemberList.None)
            .ForMember(_ => _.AgeRating, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            });

        CreateMap<PermissionValueCreateDto, PermissionValue>(MemberList.None);
        CreateMap<PermissionValueUpdateDto, PermissionValue>(MemberList.None);
        
        CreateMap<UserGroupCreateDto, UserGroup>(MemberList.None)
            .ForMember(_ => _.UserId, opt => opt.MapFrom(__ => __.TargetUserId))
            .ForMember(_ => _.Alias, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            })
            .ForMember(_ => _.Description, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            })
            .ForMember(_ => _.Priority, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            });
        CreateMap<UserGroupUpdateDto, UserGroup>(MemberList.None)
            .ForMember(_ => _.Alias, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            })
            .ForMember(_ => _.Description, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            })
            .ForMember(_ => _.Priority, opt =>
            {
                opt.Condition((_, _, _, _, context) =>
                    AutoMapperAuthorizeUserPermission(context, opt.DestinationMember.Name));
            });
        CreateMap<UserGroupActionTransferRequestCreateDto, UserGroupTransferRequest>(MemberList.None)
            .ForMember(_ => _.DestUserId, opt => opt.MapFrom(__ => __.TargetUserId));
        CreateMap<UserGroupActionInviteRequestCreateDto, UserGroupInviteRequest>(MemberList.None)
            .ForMember(_ => _.DestUserId, opt => opt.MapFrom(__ => __.TargetUserId));

        #endregion

        #region From DOMAIN to DTO

        CreateMap<Permission, PermissionReadCollectionItemResultDto>(MemberList.None);
        
        CreateMap<PermissionValue, PermissionValueCreateResultDto>(MemberList.None);
        CreateMap<PermissionValue, PermissionValueReadResultDto>(MemberList.None);
        CreateMap<PermissionValue, PermissionValueUpdateResultDto>(MemberList.None);
        CreateMap<PermissionValue, PermissionValueReadCollectionItemResultDto>(MemberList.None);

        CreateMap<File, FileCreateResultDto>(MemberList.None);
        CreateMap<File, FileUpdateResultDto>(MemberList.None);

        CreateMap<UserGroup, UserGroupCreateResultDto>(MemberList.None);
        CreateMap<UserGroup, UserGroupReadResultDto>(MemberList.None);
        CreateMap<UserGroup, UserGroupUpdateResultDto>(MemberList.None);
        CreateMap<UserGroup, UserGroupReadCollectionItemResultDto>(MemberList.None);

        CreateMap<UserGroupTransferRequest, UserGroupActionTransferRequestCreateResultDto>(MemberList.None);
        CreateMap<UserGroupInviteRequest, UserGroupActionInviteRequestReadResultDto>(MemberList.None);
        CreateMap<UserGroupInviteRequest, UserGroupActionInviteRequestReadCollectionItemResultDto>(MemberList.None);

        CreateMap<UserGroupInviteRequest, UserGroupActionInviteRequestCreateResultDto>(MemberList.None);
        CreateMap<UserGroupTransferRequest, UserGroupActionTransferRequestReadResultDto>(MemberList.None);
        CreateMap<UserGroupTransferRequest, UserGroupActionTransferRequestReadCollectionItemResultDto>(MemberList.None);

        #region EntityBase

        CreateMap<EntityBase<Guid>, IEntityBaseResultDto<Guid>>()
            .ForMember(_ => _.Id, opt => opt.MapFrom(__ => __.Id))
            .ForMember(_ => _.CreatedAt, opt => opt.MapFrom(__ => __.CreatedAt))
            .ForMember(_ => _.UpdatedAt, opt => opt.MapFrom(__ => __.UpdatedAt));

        #endregion
        
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