using System;
using System.ComponentModel.DataAnnotations;
using DTO.Models.Base;

namespace DTO.Models.UserGroup;

public class UserGroupTransferRequestReadDto : IEntityBaseDto
{
    public Guid Id { get; set; }
}