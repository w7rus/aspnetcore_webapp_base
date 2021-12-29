using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// A Permission Entity. Referenced by multiple EntityPermissionValueBase.
    /// </summary>
    public class Permission : EntityBase<Guid>
    {
        /// <summary>
        /// Alias of the permission. Unique field.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Type of the permission
        /// 0 - Unknown,
        /// 1 - Boolean,
        /// 2 - Int8, 3 - Int16, 4 - Int32, 5 - Int64,
        /// 6 - UInt8, 7 - UInt16, 8 - UInt32, 9 - UInt64,
        /// 10 - Float, 11 - Double, 12 - Decimal
        /// 13 - String,
        /// 14 - DateTime,
        /// </summary>
        public PermissionType Type { get; set; }
    }
}