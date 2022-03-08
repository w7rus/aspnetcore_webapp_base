using System;
using System.Threading;

namespace Domain.Enums.Edition_Shop;

public enum DiscountLimitType
{
    None,
    /// <summary>
    /// Accounted among all users
    /// </summary>
    Amount,
    /// <summary>
    /// Accounted per user
    /// </summary>
    AmountPerUser,
    /// <summary>
    /// Accounted among all users & reset after specified time span
    /// </summary>
    AmountTimeSpan,
    /// <summary>
    /// Accounted per user & reset after specified time span
    /// </summary>
    AmountTimeSpanPerUser,
    /// <summary>
    /// Accounted among all users & reset after specified time span
    /// </summary>
    AmountCron,
    /// <summary>
    /// Accounted per user & reset after specified time span
    /// </summary>
    AmountCronPerUser,
}