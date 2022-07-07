using System;
using DTO.Models.Base;

namespace DTO.Models.User;

public class UserReadCollectionItemResultDto : IEntityBaseResultDto<Guid>
{
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTimeOffset LastSignIn { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public string LastIpAddress { get; set; }
    public int FailedSignInAttempts { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}