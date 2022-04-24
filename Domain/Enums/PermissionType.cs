namespace Domain.Enums;

public enum PermissionType
{
    None,
    Unknown,
    Value,
    ValueNeededSelf,
    ValueNeededOthers,
    ValueNeededAny,
    ValueNeededSystem,
}