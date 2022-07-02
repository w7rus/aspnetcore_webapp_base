using System;
using System.ComponentModel.DataAnnotations;
using Common.Attributes;

namespace Domain.Entities.Base;

public abstract class EntityBase<TKey> where TKey : IEquatable<TKey>
{
    [Key]
    [AllowFilterExpression]
    [AllowFilterSort]
    public TKey Id { get; set; }

    /// <summary>
    ///     Date at which entity was created
    /// </summary>
    [AllowFilterExpression]
    [AllowFilterSort]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Date at which entity was updated
    /// </summary>
    [AllowFilterExpression]
    [AllowFilterSort]
    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsNew()
    {
        var type = typeof(TKey);
        if (type.IsValueType)
            return Activator.CreateInstance(type) is { } obj && obj.Equals(Id);

        return Id == null;
    }
}