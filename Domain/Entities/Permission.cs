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
        /// Alias of the Permission. Unique field.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Type of the Permission.
        /// Compared permissions Type must equal!
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// Compare mode of the permission. Defines how permission values are compared against each other.
        /// Compared permissions CompareMode must equal!
        /// </summary>
        public PermissionCompareMode CompareMode { get; set; }
    }
}