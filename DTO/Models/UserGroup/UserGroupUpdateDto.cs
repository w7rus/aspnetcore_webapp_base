using System;

namespace DTO.Models.UserGroup;

public class UserGroupUpdateDto
{
    public Guid Id { get; set; }
    //TODO: authorize max length
    public string Alias { get; set; }
    //TODO: authorize max length
    public string Description { get; set; }
    //TODO: l_automapper authorization (otherwise find available in range)
    public long Priority { get; set; }
    public Guid UserId { get; set; }
}