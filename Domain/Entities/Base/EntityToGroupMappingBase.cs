using System;

namespace Domain.Entities.Base
{
    public abstract class EntityToGroupMappingBase<TEntity, TGroup> : EntityBase<Guid>
        where TEntity : EntityBase<Guid>
        where TGroup : EntityBase<Guid>
    {
        public Guid EntityId { get; set; }
        public virtual TEntity Entity { get; set; }
        public Guid GroupId { get; set; }
        public virtual TGroup Group { get; set; }
    }
}