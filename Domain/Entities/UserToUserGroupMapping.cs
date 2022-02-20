using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserToUserGroupMapping : EntityToEntityMappingBase<User, UserGroup>
    {
    }
}