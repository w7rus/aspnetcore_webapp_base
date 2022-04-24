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
        /// Alias of the Permission.
        /// </summary>
        public string Alias { get; set; }

        // /// <summary>
        // /// FullAlias of the Permission. Unique field.
        // /// </summary>
        // public string FullAlias => $"{ValueType.ToString().ToLower()}_{Alias}_{Type.ToString().ToLower()}";

        /// <summary>
        /// Value Type of the Permission.
        /// Compared permissions Value Type must equal!
        /// </summary>
        public PermissionValueType ValueType { get; set; }

        /// <summary>
        /// Compare mode of the permission. Defines how permission values are compared against each other.
        /// Compared permissions CompareMode must equal!
        /// </summary>
        public PermissionCompareMode CompareMode { get; set; }
        
        /// <summary>
        /// Type of the Permission.
        /// Compared permissions Type must be one of {Self, Others, System}!
        /// </summary>
        public PermissionType Type { get; set; }
    }
}