using System;
using System.Collections.Generic;

namespace Domain.Entities.Base
{
    public abstract class EntityGroupBase<TEntity, TGroup> : EntityBase<Guid>
        where TEntity : EntityBase<Guid>
        where TGroup : EntityBase<Guid>
    {
        /// <summary>
        /// Alias of the group
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Description of the group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Entity mappings of the group
        /// Multiple-to-multiple table that links entities assigned to the group
        /// </summary>
        public virtual ICollection<EntityToEntityMappingBase<TEntity, TGroup>> GroupToEntityMappings { get; set; }

        /// <summary>
        /// Permissions value mappings of the group
        /// Multiple-to-multiple table that links permissions assigned to the group
        /// </summary>
        public virtual ICollection<EntityPermissionValueBase<TGroup>>
            PermissionValues { get; set; }
    }
}