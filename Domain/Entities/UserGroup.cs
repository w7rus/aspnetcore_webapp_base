using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserGroup : EntityGroupBase<User, UserGroup>
    {
        /// <summary>
        /// Flag indicating this group is system
        /// </summary>
        public bool IsSystem { get; set; }
    }
}