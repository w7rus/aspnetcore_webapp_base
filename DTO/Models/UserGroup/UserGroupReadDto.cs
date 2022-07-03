using System;
using System.Collections.Generic;
using Common.Models;
using Common.Models.Base;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}