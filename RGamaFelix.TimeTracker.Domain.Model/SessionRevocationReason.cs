namespace RGamaFelix.TimeTracker.Domain.Model;

/// <summary>
/// The reason why a session was revoked.
/// </summary>
public enum SessionRevocationReason
{
    /// <summary>
    /// The user requested to revoke the session.
    /// </summary>
    /// <remarks>Used when the user issues the sign-out command</remarks>
    UserRequest = 0,

    /// <summary>
    /// The session was revoked because the refresh token was expired.
    /// </summary>
    TokenExpired = 1,

    /// <summary>
    /// The session was revoked because the access token was refreshed.
    /// </summary>
    TokenRefreshed = 2,

    /// <summary>
    /// The session was revoked because the access token was replaced.
    /// </summary>
    TokenReplaced = 3
}