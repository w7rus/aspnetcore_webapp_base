using System;

namespace Common.Models;

public static class Localize
{
    public static class Error
    {
        #region Auth

        public static string UserDoesNotExistOrWrongCredentials => "UserDoesNotExistOrWrongCredentials";
        public static string UserDoesNotExistOrHttpContextMissingClaims => "UserDoesNotExistOrHttpContextMissingClaims";

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

        public static string FileSaveFailedNameChangeNotAllowedForExisting => "FileSaveFailedNameChangeNotAllowedForExisting";
        public static string FileReadFailed => "FileReadFailed";
        public static string FileDeleteFailed => "FileDeleteFailed";

        #endregion

        #region Permission

        public static string PermissionDynamicManagementNotAllowed => "PermissionDynamicManagementNotAllowed";

        public static string PermissionComparedOrCustomValueComparedRequired =>
            "PermissionComparedOrCustomValueComparedRequired";

        public static string PermissionInsufficientPermissions => "PermissionInsufficientPermissions";
        public static string PermissionDoesNotExist => "PermissionDoesNotExist";
        public static string PermissionValueDoesNotExist => "PermissionValueDoesNotExist";
        public static string PermissionValueTypeUnknown => "PermissionValueTypeUnknown";

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
        public static string UserGroupDoesNotExist => "UserGroupDoesNotExist";

        #endregion

        #region UnhandledException

        public const string UnhandledExceptionContactSystemAdministrator =
            "UnhandledExceptionContactSystemAdministrator";

        #endregion

        #region FilterMatchModel

        public static string FilterMatchModelItemValueTypeUnknown => "FilterMatchModelItemValueTypeUnknown";
        public static string FilterMatchModelItemValueTypeNotSupported => "FilterMatchModelItemValueTypeNotSupported";
        public static string FilterMatchModelPropertyUnavailable => "FilterMatchModelPropertyUnavailable";
        public static string FilterMatchModelValueTypeNotSuitable => "FilterMatchModelValueTypeNotSuitable";

        #endregion
        
        #region FilterSortModel
        
        public static string FilterSortModelPropertyUnavailable => "FilterSortModelPropertyUnavailable";

        #endregion

        #region DbContext

        public static string DbProviderNotSupported => "DbProviderNotSupported";

        #endregion
    }

    public static class Warning
    {
        public static string XssVulnerable => "XSSVulnerable";
    }

    public static class Log
    {
        #region Middleware

        public static string MiddlewareForwardStart(Type type) =>
            $"[Middleware {type.Name}] (Forward-Start)";

        public static string MiddlewareForwardEnd(Type type) => 
            $"[Middleware {type.Name}] (Forward-End)";

        public static string MiddlewareBackwardStart(Type type) =>
            $"[Middleware {type.Name}] (Backward-Start)";

        public static string MiddlewareBackwardEnd(Type type) =>
            $"[Middleware {type.Name}] (Backward-End)";

        #endregion

        #region Method

        public static string MethodStart(Type type, string methodName) =>
            $"[{type.Name}.{methodName}] (Start)";

        public static string Method(Type type, string methodName, string message) =>
            $"[{type.Name}.{methodName}] {message}";

        public static string MethodEnd(Type type, string methodName) =>
            $"[{type.Name}.{methodName}] (End)";

        public static string MethodError(Type type, string methodName, string message) =>
            $"[{type.Name}.{methodName}] (Error) {Environment.NewLine + message}";

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