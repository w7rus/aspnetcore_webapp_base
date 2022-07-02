using System;
using Domain.Entities.Base;

namespace DTO.Models.Base;

public class EntityBaseFSPCollectionItemResultDto<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}