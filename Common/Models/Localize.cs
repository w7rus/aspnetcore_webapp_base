using System;

namespace Common.Models;

public static class Localize
{
    public static class Error
    {
        #region Auth

        public static string AuthSignUpFailed => "AuthSignUpFailed";
        public static string AuthSignInFailed => "AuthSignInFailed";
        public static string AuthRefreshFailed => "AuthRefreshFailed";
        public static string AuthSignOutFailed => "AuthSignOutFailed";

        #endregion

        #region User

        public static string UserIdRetrievalFailed => "UserIdRetrievalFailed";

        #endregion

        #region JsonWebToken

        public static string JsonWebTokenIdRetrievalFailed => "JsonWebTokenIdRetrievalFailed";
        public static string JsonWebTokenNotProvided => "JsonWebTokenNotProvided";
        public static string JsonWebTokenNotFound => "JsonWebTokenNotFound";
        public static string JsonWebTokenExpired => "JsonWebTokenExpired";
        public static string JsonWebTokenValidationFailed => "JsonWebTokenValidationFailed";

        #endregion

        #region RefreshToken

        public static string RefreshTokenNotProvided => "RefreshTokenNotProvided";
        public static string RefreshTokenNotFound => "RefreshTokenNotFound";
        public static string RefreshTokenExpired => "RefreshTokenExpired";
        public static string RefreshTokenIncorrectFormat => "RefreshTokenIncorrectFormat";

        #endregion

        #region File

        public static string FileCreateFailed => "FileCreateFailed";
        public static string FileReadFailed => "FileReadFailed";
        public static string FileDeleteFailed => "FileDeleteFailed";

        #endregion

        #region Permission

        public static string PermissionDynamicManagementNotAllowed => "PermissionDynamicManagementNotAllowed";

        public static string PermissionAuthorizePermissionValueAtLeastOneRequired =>
            "PermissionAuthorizePermissionValueAtLeastOneRequired";

        public static string PermissionInsufficientPermissions => "PermissionInsufficientPermissions";

        #endregion

        #region HttpClient

        public static string ResponseStatusCodeUnsuccessful => "ResponseStatusCodeUnsuccessful";

        #endregion

        #region Misc

        public static string ObjectDeserializationFailed => "ObjectDeserializationFailed";
        public static string ObjectCastFailed => "ObjectCastFailed";
        public static string DependencyInjectionFailed => "DependencyInjectionFailed";
        public static string ValueRetrievalFailed => "ValueRetrievalFailed";

        #endregion

        #region UserGroup

        public static string UserGroupIsSystemManagementNotAllowed => "UserGroupIsSystemManagementNotAllowed";

        #endregion

        #region UnhandledException

        public const string UnhandledExceptionContactSystemAdministrator = "UnhandledExceptionContactSystemAdministrator";

        #endregion
    }

    public static class Warning
    {
        public static string XssVulnerable => "XSSVulnerable";
    }

    public static class Log
    {
        #region Middleware

        public static string MiddlewareForwardStart(string assemblyName) =>
            $"[Middleware] {assemblyName} (Forward-Start)";

        public static string MiddlewareForwardEnd(string assemblyName) => $"[Middleware] {assemblyName} (Forward-End)";

        public static string MiddlewareBackwardStart(string assemblyName) =>
            $"[Middleware] {assemblyName} (Backward-End)";

        public static string MiddlewareBackwardEnd(string assemblyName) =>
            $"[Middleware] {assemblyName} (Backward-End)";

        #endregion

        #region Method

        public static string MethodStart(string assemblyName, string methodName) =>
            $"[{assemblyName}.{methodName}] (Start)";
        
        public static string Method(string assemblyName, string methodName, string message) =>
            $"[{assemblyName}.{methodName}] {message}";

        public static string MethodEnd(string assemblyName, string methodName) =>
            $"[{assemblyName}.{methodName}] (End)";

        public static string MethodError(string assemblyName, string methodName, string message) =>
            $"[{assemblyName}.{methodName}] (Error) {Environment.NewLine + message}";
        
        public static string UnhandledMethodError(string traceId, string message) =>
            $"[{traceId}] (UnhandledError) {Environment.NewLine + message}";

        #endregion

        #region Background Service

        public static string BackgroundServiceStarting(string assemblyName) =>
            $"[{assemblyName}] (Starting)";

        public static string BackgroundServiceStopping(string assemblyName) =>
            $"[{assemblyName}] (End)";

        public static string BackgroundServiceWorking(string assemblyName) =>
            $"[{assemblyName}] (Working)";

        public static string BackgroundServiceError(string assemblyName, string message) =>
            $"[{assemblyName}] (Error) {Environment.NewLine + message}";

        #endregion

        #region Job

        public static string JobExecuted(string assemblyName) =>
            $"[{assemblyName}] Executed";
        
        public static string JobAborted(string assemblyName) =>
            $"[{assemblyName}] Aborted";

        public static string JobCompleted(string assemblyName) =>
            $"[{assemblyName}] Completed";

        public static string JobError(string assemblyName, string message) =>
            $"[{assemblyName}] Error {Environment.NewLine + message}";

        #endregion
    }
}