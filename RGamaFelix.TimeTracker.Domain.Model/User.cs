using System.Net;
using Microsoft.AspNetCore.Identity;

namespace RGamaFelix.TimeTracker.Domain.Model;

public class User : IdentityUser<Guid>, IEntityBase
{
    private readonly List<Audit> _audits;
    private readonly List<Session> _sessions;

    private User()
    {
        _sessions = new List<Session>();
        _audits = new List<Audit>();
    }

    public IReadOnlyCollection<Audit>? Audits => _audits?.AsReadOnly();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    public Session AddSession(string accessToken, DateTime accessTokenExpiresAt, string refreshToken,
        DateTime refreshTokenExpiresAt, IPAddress requestIp)
    {
        var session = Session.Create(this, accessToken, accessTokenExpiresAt, refreshToken, refreshTokenExpiresAt,
            requestIp);
        _sessions.Add(session);
        return session;
    }

    public static User Create(string name, string email)
    {
        return new User { UserName = name, Email = email, EmailConfirmed = true };
    }

    /// <summary>
    ///     Replaces the current session with a new one.
    /// </summary>
    /// <param name="currentSession"></param>
    /// <param name="accessToken"></param>
    /// <param name="accessTokenExpireDate"></param>
    /// <param name="refreshToken"></param>
    /// <param name="refreshTokenExpireDate"></param>
    /// <param name="ipAddress"></param>
    public void ReplaceSession(Session currentSession, string accessToken, DateTime accessTokenExpireDate,
        string refreshToken, DateTime refreshTokenExpireDate, IPAddress ipAddress)
    {
        var newSession = AddSession(accessToken, accessTokenExpireDate, refreshToken, refreshTokenExpireDate,
            ipAddress);
        currentSession.Revoke(SessionRevocationReason.TokenReplaced, newSession);
    }
}
