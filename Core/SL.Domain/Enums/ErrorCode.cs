using System;
namespace SL.Domain.Enums;

public enum ErrorCode
{
    None = 0,
    InternalServerError = 500,
    DependencyFailure = 503,
    UnauthorizedAccess = 200,
    AuthenticationFailed = 201,
    AccountLocked = 202,
    EmailNotConfirmed = 203,
    ValidationFailure = 400,
    ResourceNotFound = 404,
    DuplicateResource = 409,
    BusinessRuleViolation = 410
}
