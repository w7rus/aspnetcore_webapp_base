using System;

namespace Domain.Entities.Base
{
    public abstract class EntityPermissionValueBase<TEntity> : EntityBase<Guid> where TEntity : EntityBase<Guid>
    {
        /// <summary>
        /// Amount of power associated with the action
        /// </summary>
        public byte[] Value { get; set; }

        // ///TODO: Grant supposed to be a limit of what action can set as Value with their permission set (if they are allowed to edit permissions)
        // /// <summary>
        // /// Amount of power needed to modify this permission value
        // /// Compared with uint64_g_any_a_update_o_permissionvalue_power
        // /// </summary>
        // public ulong Grant { get; set; }

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