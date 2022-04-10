using System;
using System.Collections.Generic;

namespace Domain.Entities.Base
{
    public abstract class EntityGroupBase<TEntity, TGroup> : EntityBase<Guid>
        where TEntity : EntityBase<Guid>
        where TGroup : EntityBase<Guid>
    {
        /// <summary>
        /// Alias of the Group
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Description of the Group
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Group mappings to Entities
        /// </summary>
        public virtual ICollection<EntityToEntityMappingBase<TEntity, TGroup>> EntityToEntityGroupMappings { get; set; }

        /// <summary>
        /// Group mappings to PermissionValues
        /// </summary>
        public virtual ICollection<EntityPermissionValueBase<TGroup>> PermissionValues { get; set; }
    }
}