using System;

namespace Domain.Entities.Base
{
    public abstract class EntityToEntityMappingBase<TEntityLeft, TEntityRight> : EntityBase<Guid>
        where TEntityLeft : EntityBase<Guid>
        where TEntityRight : EntityBase<Guid>
    {
        public Guid EntityLeftId { get; set; }
        public virtual TEntityLeft EntityLeft { get; set; }
        public Guid EntityRightId { get; set; }
        public virtual TEntityRight EntityRight { get; set; }
    }
}