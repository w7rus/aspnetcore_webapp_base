namespace Common.Enums;

public enum ErrorType
{
    None,
    Generic,
    Unhandled,
    ModelState,
    HttpContext,
    Auth,
    Permission,
    HttpClient,
    Request
}