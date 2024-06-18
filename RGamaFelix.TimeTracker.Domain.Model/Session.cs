using System.Net;

namespace RGamaFelix.TimeTracker.Domain.Model;

/// <summary>
///     Represents a session of a user.
/// </summary>
public class Session : IEntityBase
{
    private Session()
    { }

    /// <summary>
    ///     Get the access token of the session.
    /// </summary>
    public string AccessToken { get; private set; }

    /// <summary>
    ///     Get the date and time when the access token expires.
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; private set; }

    /// <summary>
    ///     Get the date and time when the session was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    ///     Indicates whether the session was revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>
    ///     Get the refresh token of the session.
    /// </summary>
    public string RefreshToken { get; private set; }

    /// <summary>
    ///     Get the date and time when the refresh token expires.
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; private set; }

    public Session? ReplacedBy { get; private set; }
    public Guid? ReplacedById { get; }

    /// <summary>
    ///     Get the IP address of the request
    /// </summary>
    public IPAddress RequestIp { get; private set; }

    /// <summary>
    ///     Get the reason why the session was revoked.
    /// </summary>
    public SessionRevocationReason? RevocationReason { get; private set; }

    /// <summary>
    ///     Get the date and time when the session was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    ///     Get the user that owns the session.
    /// </summary>
    public User User { get; private set; }

    /// <summary>
    ///     Get the Id of the user that owns the session.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    ///     Get the unique identifier of the session.
    /// </summary>
    public Guid Id { get; private set; }

    public static Session Create(User user, string accessToken, DateTime accessTokenExpiresAt, string refreshToken,
        DateTime refreshTokenExpiresAt, IPAddress requestIp)
    {
        var session = new Session
        {
            Id = Guid.NewGuid(),
            User = user,
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            CreatedAt = DateTime.UtcNow,
            RequestIp = requestIp
        };
        return session;
    }

    public Session Revoke(SessionRevocationReason reason, Session? newSession)
    {
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        ReplacedBy = newSession;
        return this;
    }
}
