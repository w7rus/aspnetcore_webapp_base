using System;

namespace Common.Models;

public static class Localize
{
    public static class Error
    {
        #region UnhandledException

        public const string UnhandledExceptionContactSystemAdministrator =
            "UnhandledExceptionContactSystemAdministrator";

        #endregion

        #region HttpClient

        public static string ResponseStatusCodeUnsuccessful => "ResponseStatusCodeUnsuccessful";

        #endregion

        #region FilterSortModel

        public static string FilterSortModelPropertyNotFoundOrUnavailable =>
            "FilterSortModelPropertyNotFoundOrUnavailable";

        #endregion

        #region DbContext

        public static string DbProviderNotSupported => "DbProviderNotSupported";

        #endregion

        #region Auth

        public static string UserDoesNotFoundOrWrongCredentials => "UserDoesNotFoundOrWrongCredentials";
        public static string UserDoesNotFoundOrHttpContextMissingClaims => "UserDoesNotFoundOrHttpContextMissingClaims";

        #endregion

        #region User

        public static string UserIdRetrievalFailed => "UserIdRetrievalFailed";
        public static string UserNotFound => "UserNotFound";
        public static string UserAlreadyClaimed => "UserAlreadyClaimed";

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

        public static string FileSaveFailedNameChangeNotAllowedForExisting =>
            "FileSaveFailedNameChangeNotAllowedForExisting";

        public static string FileNotFound => "FileNotFound";
        public static string FileReadFailed => "FileReadFailed";
        public static string FileDeleteFailed => "FileDeleteFailed";

        #endregion

        #region Permission

        public static string PermissionDynamicManagementNotAllowed => "PermissionDynamicManagementNotAllowed";

        public static string PermissionComparedOrCustomValueComparedRequired =>
            "PermissionComparedOrCustomValueComparedRequired";

        public static string PermissionInsufficientPermissions => "PermissionInsufficientPermissions";
        public static string PermissionNotFound => "PermissionDoesNotExist";
        public static string PermissionValueNotFound => "PermissionValueDoesNotExist";
        public static string PermissionValueTypeUnknown => "PermissionValueTypeUnknown";

        #endregion

        #region UserGroup

        public static string UserGroupIsSystemManagementNotAllowed => "UserGroupIsSystemManagementNotAllowed";
        public static string UserGroupNotFound => "UserGroupDoesNotExist";

        #endregion

        #region FilterMatchModel

        public static string FilterMatchModelItemExpressionValueTypeNotSupported =>
            "FilterMatchModelItemExpressionValueTypeNotSupported";

        public static string FilterMatchModelItemExpressionValueFailedToParseGuid =>
            "FilterMatchModelItemExpressionValueFailedToParseGuid";

        public static string FilterMatchModelPropertyNotFoundOrUnavailable =>
            "FilterMatchModelPropertyNotFoundOrUnavailable";

        public static string FilterMatchModelValueTypeNotCompatible => "FilterMatchModelValueTypeNotCompatible";

        public static string FilterMatchModelItemFirstExpressionLogicalOperationNoneNotOnly =>
            "FilterMatchModelItemFirstExpressionLogicalOperationNoneNotOnly";

        public static string FilterMatchModelItemNotFirstExpressionLogicalOperationAndOrOnly =>
            "FilterMatchModelItemNotFirstExpressionLogicalOperationAndOrOnly";

        #endregion

        #region Request

        public const string RequestMultipartExpected = "RequestMultipartExpected";
        public const string RequestContentTypeBoundaryNotFound = "RequestContentTypeBoundaryNotFound";
        public const string RequestMultipartBoundaryLengthExceedsLimit = "RequestMultipartBoundaryLengthExceedsLimit";
        public const string RequestMultipartSectionEncodingNotSupported = "RequestMultipartSectionEncodingNotSupported";
        public const string RequestMultipartSectionNotFound = "RequestMultipartSectionNotFound";

        public const string RequestMultipartSectionContentDispositionParseFailed =
            "RequestMultipartSectionContentDispositionParseFailed";

        public const string RequestMultipartSectionContentDispositionFileExpected =
            "RequestMultipartSectionContentDispositionFileExpected";

        public const string RequestMultipartSectionContentDispositionFormExpected =
            "RequestMultipartSectionContentDispositionFormExpected";

        #endregion

        #region Generic

        public static string TypeNotFound => "TypeNotFound";
        public static string ObjectDeserializationFailed => "ObjectDeserializationFailed";
        public static string ObjectCastFailed => "ObjectCastFailed";
        public static string DependencyInjectionFailed => "DependencyInjectionFailed";
        public static string ValueRetrievalFailed => "ValueRetrievalFailed";

        #endregion
    }

    public static class Warning
    {
        public static string XssVulnerable => "XSSVulnerable";
    }

    public static class Log
    {
        #region Middleware

        public static string MiddlewareForwardStart(Type type)
        {
            return $"[Middleware {type.Name}] (Forward-Start)";
        }

        public static string MiddlewareForwardEnd(Type type)
        {
            return $"[Middleware {type.Name}] (Forward-End)";
        }

        public static string MiddlewareBackwardStart(Type type)
        {
            return $"[Middleware {type.Name}] (Backward-Start)";
        }

        public static string MiddlewareBackwardEnd(Type type)
        {
            return $"[Middleware {type.Name}] (Backward-End)";
        }

        #endregion

        #region Method

        public static string MethodStart(Type type, string methodName)
        {
            return $"[{type.Name}.{methodName}] (Start)";
        }

        public static string Method(Type type, string methodName, string message)
        {
            return $"[{type.Name}.{methodName}] {message}";
        }

        public static string MethodEnd(Type type, string methodName)
        {
            return $"[{type.Name}.{methodName}] (End)";
        }

        public static string MethodError(Type type, string methodName, string message)
        {
            return $"[{type.Name}.{methodName}] (Error) {Environment.NewLine + message}";
        }

        #endregion

        #region Background Service

        public static string BackgroundServiceStarting(string assemblyName)
        {
            return $"[{assemblyName}] (Starting)";
        }

        public static string BackgroundServiceStopping(string assemblyName)
        {
            return $"[{assemblyName}] (End)";
        }

        public static string BackgroundServiceWorking(string assemblyName)
        {
            return $"[{assemblyName}] (Working)";
        }

        public static string BackgroundServiceError(string assemblyName, string message)
        {
            return $"[{assemblyName}] (Error) {Environment.NewLine + message}";
        }

        #endregion

        #region Job

        public static string JobExecuted(string assemblyName)
        {
            return $"[{assemblyName}] Executed";
        }

        public static string JobAborted(string assemblyName)
        {
            return $"[{assemblyName}] Aborted";
        }

        public static string JobCompleted(string assemblyName)
        {
            return $"[{assemblyName}] Completed";
        }

        public static string JobError(string assemblyName, string message)
        {
            return $"[{assemblyName}] Error {Environment.NewLine + message}";
        }

        #endregion
    }
}