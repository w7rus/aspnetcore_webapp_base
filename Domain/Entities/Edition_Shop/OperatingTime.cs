using System;
using Domain.Entities.Base;

namespace Domain.Entities.Edition_Shop;

public class OperatingTime : EntityBase<Guid>
{
    /// <summary>
    /// Date from Store should be Open
    /// </summary>
    public DateTimeOffset OpensAt { get; set; }

    /// <summary>
    /// Date from Store should be Closed
    /// </summary>
    public DateTimeOffset ClosesAt { get; set; }

    /// <summary>
    /// Date from Store should be accepting Orders
    /// </summary>
    public DateTimeOffset OrdersFrom { get; set; }

    /// <summary>
    /// Date from Store shouldn't be accepting Orders
    /// </summary>
    public DateTimeOffset OrdersUntil { get; set; }

    /// <summary>
    /// DayOfWeek of a OperatingTime
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Is OperatingTime Workday?
    /// </summary>
    public bool IsWorkday { get; set; }

    /// <summary>
    /// Id of a Store this OperatingTime references
    /// </summary>
    public Guid StoreId { get; set; }

    /// <summary>
    /// [Proxy]
    /// Store this OperatingTime references
    /// </summary>
    public virtual Store Store { get; set; }
}