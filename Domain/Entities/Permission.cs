using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Attributes;
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
        [AllowFilterExpression]
        [AllowFilterSort]
        public string Alias { get; set; }

        /// <summary>
        /// Compare mode of the permission. Defines how permission values are compared against each other.
        /// Compared permissions CompareMode must equal!
        /// </summary>
        [AllowFilterExpression]
        [AllowFilterSort]
        public PermissionCompareMode CompareMode { get; set; }

        /// <summary>
        /// Type of the Permission.
        /// Compared permissions Type must be one of {Self, Others, System}!
        /// </summary>
        [AllowFilterExpression]
        [AllowFilterSort]
        public PermissionType Type { get; set; }
    }
}