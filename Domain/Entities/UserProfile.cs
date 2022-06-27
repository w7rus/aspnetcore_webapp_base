using System;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserProfile : EntityBase<Guid>
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}