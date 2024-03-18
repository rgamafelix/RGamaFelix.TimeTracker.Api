using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Domain.Service;
using RGamaFelix.TimeTracker.Domain.Service.Handler;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;

public class CreateRegularUserHandler : ValidatedRequestHandler<CreateRegularUserRequest, CreateUserResponse>
{
    private readonly TimeTrackerDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IAuthenticationResolver _authenticationResolver;
    private readonly HttpContext _httpContext;

    public CreateRegularUserHandler
    (IValidator<CreateRegularUserRequest> validator, ILogger<CreateRegularUserHandler> logger,
        TimeTrackerDbContext dbContext,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor, IAuthenticationResolver authenticationResolver) : base(validator,
        logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _authenticationResolver = authenticationResolver;
        _httpContext = httpContextAccessor.HttpContext;
    }

    protected override async Task<IServiceResultOf<CreateUserResponse>> HandleValidatedRequest(
        CreateRegularUserRequest request, CancellationToken cancellationToken)
    {
        if (!_authenticationResolver.Resolve(true, new[] { "Admin" }))
        {
            return ServiceResultOf<CreateUserResponse>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
        }

        if (await _userManager.Users.AnyAsync(
                u => u.NormalizedUserName.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase) ||
                     u.NormalizedEmail.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken: cancellationToken))
        {
            Logger.LogWarning("User {UserName}/{UserEmail} already exists", request.Name, request.Email);

            return ServiceResultOf<CreateUserResponse>.Fail(UserAlreadyExists, ResultTypeCode.Multiplicity);
        }

        var newUser = User.Create(
            request.Name, request.Email
        );

        var createUserResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {
            Logger.LogError("Error creating user {UserName}: {Errors}", request.Name,
                string.Join(',', createUserResult.Errors.Select(e => $"{e.Code}: {e.Description}")));
            return ServiceResultOf<CreateUserResponse>.Fail(createUserResult.Errors.Select(e => e.Code),
                ResultTypeCode.GenericError);
        }

        var addToRoleIdentityResult = await _userManager.AddToRoleAsync(newUser, "Regular");
        if (!addToRoleIdentityResult.Succeeded)
        {
            Logger.LogError("Error adding user {UserName} to role: {Errors}", request.Name,
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

    public const string UserAlreadyExists = "UserAlreadyExists";
}