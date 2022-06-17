using System;
using Common.Attributes;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class PermissionValue : EntityBase<Guid>
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
    }
}