using System;
using System.Collections.Generic;

namespace Domain.Entities.Base
{
    public abstract class EntityGroupBase<TEntity, TGroup> : EntityBase<Guid>
        where TEntity : EntityBase<Guid>
        where TGroup : EntityBase<Guid>
    {
        /// <summary>
        /// Alias of the EntityGroup
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Description of the EntityGroup
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// [Proxy]
        /// Entities to EntityGroup mappings
        /// </summary>
        public virtual ICollection<EntityToEntityMappingBase<TEntity, TGroup>> EntityToEntityGroupMappings { get; set; }

        /// <summary>
        /// [Proxy]
        /// PermissionValues referencing this EntityGroup
        /// </summary>
        public virtual ICollection<EntityPermissionValueBase<TGroup>> PermissionValues { get; set; }
    }
}