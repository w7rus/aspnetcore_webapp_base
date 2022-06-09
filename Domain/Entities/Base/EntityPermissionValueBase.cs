using System;
using Common.Attributes;

namespace Domain.Entities.Base
{
    //TODO: Remove generic and rely on code logic for working with PermissionValue of TEntity
    public abstract class EntityPermissionValueBase<TEntity> : EntityBase<Guid> where TEntity : EntityBase<Guid>
    {
        /// <summary>
        /// Amount of power associated with the action
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// A Permission to which permission value bound to
        /// </summary>
        [AllowFilterExpression]
        [AllowFilterSort]
        public Guid PermissionId { get; set; }

        /// <summary>
        /// [Proxy]
        /// A Permission to which permission value bound to
        /// </summary>
        public virtual Permission Permission { get; set; }

        /// <summary>
        /// An Entity to which permission value bound to
        /// </summary>
        [AllowFilterExpression]
        [AllowFilterSort]
        public Guid EntityId { get; set; }

        /// <summary>
        /// [Proxy]
        /// An Entity to which permission value bound to
        /// </summary>
        public virtual TEntity Entity { get; set; }
    }
}