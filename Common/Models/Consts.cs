namespace Common.Models;

public static class Consts
{
    public const ulong RootUserGroupValue = ulong.MaxValue;
    public const ulong RootUserGroupPriority = ulong.MaxValue;
    public const ulong MemberUserGroupValue = 50ul;
    public const ulong MemberUserGroupPriority = 50ul;
    public const ulong GuestUserGroupValue = 25ul;
    public const ulong GuestUserGroupPriority = 25ul;
    public const ulong BannedUserGroupValue = 0ul;
    public const ulong BannedUserGroupPriority = ulong.MaxValue - 1;
    public const string AutoMapperModelAuthorizeDataKey = "AutoMapperModelAuthorizeData";
}