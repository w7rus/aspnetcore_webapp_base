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
                    User_UserGroup_AuthorizePermission(context, objMemberTo.GetType().Name));
                // options.MapFrom(__ => __.AgeRating);
            });

        #endregion
    }

    #endregion

    #region Utilities
    
    bool User_UserGroup_AuthorizePermission(ResolutionContext context, string objMemberToName)
    {
        var userToUserGroupService = (context
                                          .Options
                                          .ServiceCtor(typeof(IUserToUserGroupService)) ??
                                      throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
                                     IUserToUserGroupService ??
                                     throw new CustomException(Localize.Error.ObjectCastFailed);

        //Get user from the context items
        if (!context.Items.TryGetValue("user", out var userObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " + nameof(userObject));
        var user = userObject as User;

        var result = true;

        //Get dictionary of Permission + PermissionValue + PermissionValueSystem tuples
        context.Items.TryGetValue("modelFieldMappingPermissionAuthorizationTuples",
            out var modelFieldMappingPermissionAuthorizationTuplesObject);

        var modelFieldMappingPermissionAuthorizationTuples =
            modelFieldMappingPermissionAuthorizationTuplesObject as
                Dictionary<string, AutoMapperComparePermissionToTuple> ?? throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(modelFieldMappingPermissionAuthorizationTuplesObject));

        //Get Permission + PermissionValue + PermissionValueSystem tuple for corresponding model field
        modelFieldMappingPermissionAuthorizationTuples.TryGetValue(objMemberToName,
            out var autoMapperComparePermissionToPermissionValueTuple);

        if (autoMapperComparePermissionToPermissionValueTuple == null)
            throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(autoMapperComparePermissionToPermissionValueTuple));

        //Get Permission
        var comparedPermission = autoMapperComparePermissionToPermissionValueTuple.ComparedPermission as Permission;
        if (comparedPermission == null)
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " + nameof(comparedPermission));

        //Get PermissionValue
        var comparablePermissionValue =
            autoMapperComparePermissionToPermissionValueTuple.ComparablePermissionValue as dynamic;
        
        //Get PermissionValueSystem
        var comparablePermissionValueSystem =
            autoMapperComparePermissionToPermissionValueTuple.ComparablePermissionValueSystem as dynamic;

        //Check that at least one exists
        if (comparablePermissionValue == null && comparablePermissionValueSystem == null)
            throw new CustomException(Localize.Error.PermissionAuthorizePermissionValueAtLeastOneRequired);

        //Authorize User with all PermissionValues from their UserGroups against given PermissionValue AND PermissionValueSystem
        if (comparablePermissionValue != null)
            result &= userToUserGroupService.AuthorizePermission(user, comparedPermission, comparablePermissionValue)
                .ConfigureAwait(false).GetAwaiter().GetResult();

        if (comparablePermissionValueSystem != null)
            result &= userToUserGroupService
                .AuthorizePermission(user, comparedPermission, comparablePermissionValueSystem)
                .ConfigureAwait(false).GetAwaiter().GetResult();

        //Return the result
        return result;
    }

    bool User_UserGroup_AuthorizePermission_CustomValue(ResolutionContext context, string objMemberToName)
    {
        var userToUserGroupService = (context
                                          .Options
                                          .ServiceCtor(typeof(IUserToUserGroupService)) ??
                                      throw new CustomException(Localize.Error.DependencyInjectionFailed)) as
                                     IUserToUserGroupService ??
                                     throw new CustomException(Localize.Error.ObjectCastFailed);

        //Get user from the context items
        if (!context.Items.TryGetValue("user", out var userObject))
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " + nameof(userObject));
        var user = userObject as User;

        //Get dictionary of Permission + CustomValue tuples
        context.Items.TryGetValue("modelFieldMappingPermissionAuthorizationTuples",
            out var modelFieldMappingPermissionAuthorizationTuplesObject);

        var modelFieldMappingPermissionAuthorizationTuples =
            modelFieldMappingPermissionAuthorizationTuplesObject as
                Dictionary<string, AutoMapperComparePermissionToTuple> ?? throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(modelFieldMappingPermissionAuthorizationTuplesObject));

        //Get Permission + CustomValue tuple for corresponding model field
        modelFieldMappingPermissionAuthorizationTuples.TryGetValue(objMemberToName,
            out var autoMapperComparePermissionToPermissionValueTuple);

        if (autoMapperComparePermissionToPermissionValueTuple == null)
            throw new CustomException(
                Localize.Error.ValueRetrievalFailed + " " + nameof(autoMapperComparePermissionToPermissionValueTuple));

        //Get Permission
        var comparedPermission = autoMapperComparePermissionToPermissionValueTuple.ComparedPermission as Permission;
        if (comparedPermission == null)
            throw new CustomException(Localize.Error.ValueRetrievalFailed + " " + nameof(comparedPermission));

        //Get CustomValue
        var comparableCustomValue =
            autoMapperComparePermissionToPermissionValueTuple.ComparableCustomValue;
        
        //Return result of Authorize User with all PermissionValues from their UserGroups against given CustomValue
        return userToUserGroupService.AuthorizePermission(user, comparedPermission, comparableCustomValue)
        .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    #endregion
}