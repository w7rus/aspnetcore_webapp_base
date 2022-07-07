using System;

namespace DTO.Models.Base;

public interface IEntityBaseResultDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}