using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Base;

namespace BLL.Services.Advanced;

public interface IUserToGroupService
{
    Task<ICollection<UserGroup>> GetUserGroupsByUser(User user);
    Task<ICollection<User>> GetUsersByUserGroup(UserGroup userGroup);

    Task AuthorizePermission(
        Permission permission,
        UserGroupPermissionValue userGroupPermissionValue,
        EntityPermissionValueBase<EntityBase<Guid>> entityPermissionValue
    );
}

public class UserToGroupService
{
    
}