namespace Common.Models;

public class RecurringJobId
{
    public const string JsonWebTokenPurge = "Hangfire_RJ_JsonWebToken_Purge";
    public const string RefreshTokenPurge = "Hangfire_RJ_RefreshToken_Purge";
    public const string AuthorizePurge = "Hangfire_RJ_AuthorizePurge_Purge";
    public const string UsersPurge = "Hangfire_RJ_Users_Purge";
}