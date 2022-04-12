namespace Common.Models;

public static class Consts
{
    public const ulong RootUserGroupPowerBase = ulong.MaxValue;
    public const ulong RootUserGroupPriorityBase = ulong.MaxValue;
    public const ulong MemberUserGroupPowerBase = 50ul;
    public const ulong MemberUserGroupPriorityBase = 50ul;
    public const ulong GuestUserGroupPowerBase = 25ul;
    public const ulong GuestUserGroupPriorityBase = 25ul;
    public const ulong BannedUserGroupPowerBase = 0ul;
    public const ulong BannedUserGroupPriorityBase = ulong.MaxValue - 1;
}