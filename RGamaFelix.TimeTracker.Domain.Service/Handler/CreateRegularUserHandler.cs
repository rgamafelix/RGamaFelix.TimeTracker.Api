using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class CreateRegularUserHandler : IRequestHandler<CreateRegularUserRequest, IServiceResultOf<CreateUserResponse>>
{
    public const string UserAlreadyExists = "UserAlreadyExists";
    private readonly IAuthenticationResolver _authenticationResolver;
    private readonly TimeTrackerDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly ILogger<CreateRegularUserHandler> _logger;
    private readonly UserManager<User> _userManager;

    public CreateRegularUserHandler(ILogger<CreateRegularUserHandler> logger, TimeTrackerDbContext dbContext,
        UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        IAuthenticationResolver authenticationResolver)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
        _authenticationResolver = authenticationResolver;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<IServiceResultOf<CreateUserResponse>> Handle(CreateRegularUserRequest request,
        CancellationToken cancellationToken)
    {
        if (await _userManager.Users.AnyAsync(
                u => u.NormalizedUserName.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase) ||
                    u.NormalizedEmail.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken))
        {
            _logger.LogWarning("User {UserName}/{UserEmail} already exists", request.Name, request.Email);
            return ServiceResultOf<CreateUserResponse>.Fail(UserAlreadyExists, ResultTypeCode.Multiplicity);
        }

        var newUser = User.Create(request.Name, request.Email);
        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {
            _logger.LogError("Error creating user {UserName}: {Errors}", request.Name,
                string.Join(',', createUserResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return ServiceResultOf<CreateUserResponse>.Fail(createUserResult.Errors.Select(e => e.Code),
                ResultTypeCode.GenericError);
        }

        var addToRoleIdentityResult = await _userManager.AddToRoleAsync(newUser, "Regular");
        if (!addToRoleIdentityResult.Succeeded)
        {
            _logger.LogError("Error adding user {UserName} to role: {Errors}", request.Name,
                string.Join(',', createUserResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return ServiceResultOf<CreateUserResponse>.Fail(addToRoleIdentityResult.Errors.Select(e => e.Code),
                ResultTypeCode.GenericError);
        }

        var loggedUser = await _userManager.GetUserAsync(_httpContext.User);
        var audit = Audit.Create(loggedUser!, AuditAction.Created, newUser.Id, newUser.GetType().Name,
            $"Regular user {newUser.UserName} created.");
        _dbContext.Audits.Add(audit);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResultOf<CreateUserResponse>.Success(
            new CreateUserResponse(newUser.Id, newUser.UserName, newUser.Email, new[] { "Regular" }),
            ResultTypeCode.Ok);
    }
}