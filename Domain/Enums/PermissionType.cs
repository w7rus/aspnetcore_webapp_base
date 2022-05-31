namespace Domain.Enums;

public enum PermissionType
{
    None,
    Unknown,
    Value,
    ValueNeededOwner,
    ValueNeededOthers,
    ValueNeededAny,
    ValueNeededSystem,
}