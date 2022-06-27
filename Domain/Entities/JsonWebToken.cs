using System;
using Domain.Entities.Base;

namespace Domain.Entities;

public class JsonWebToken : EntityBase<Guid>
{
    public string Token { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset DeleteAfter { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; }
}