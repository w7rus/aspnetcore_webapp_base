using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Base
{
    public abstract class EntityBase<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsNew()
        {
            var type = typeof(TKey);
            if (type.IsValueType)
                return Activator.CreateInstance(type) is { } obj && obj.Equals(Id);

            return Id == null;
        }
    }
}