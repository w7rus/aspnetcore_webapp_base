using System;

namespace Domain.Entities.Base
{
    public abstract class EntityPermissionValueBase<TEntity> : EntityBase<Guid> where TEntity : EntityBase<Guid>
    {
        /// <summary>
        /// Amount of power associated with the action
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// Amount of power needed to modify this permission value
        /// Compared with uint64_any_modify_permission_power
        /// </summary>
        public ulong Grant { get; set; }

        /// <summary>
        /// A Permission to which permission value bound to
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// [Proxy]
        /// A Permission to which permission value bound to
        /// </summary>
        public virtual Permission Permission { get; set; }

        /// <summary>
        /// An Entity to which permission value bound to
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// [Proxy]
        /// An Entity to which permission value bound to
        /// </summary>
        public virtual TEntity Entity { get; set; }
    }
}