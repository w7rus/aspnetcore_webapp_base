using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserGroup : EntityGroupBase<User, UserGroup>
    {
    }
}