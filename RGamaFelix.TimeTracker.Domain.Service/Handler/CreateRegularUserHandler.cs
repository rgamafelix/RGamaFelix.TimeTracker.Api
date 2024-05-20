using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Domain.Model;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Request.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class CreateRegularUserHandler : IRequestHandler<CreateRegularUserRequest, IServiceResultOf<CreateUserResponse>>
{
    public const string UserAlreadyExists = "UserAlreadyExists";
    private readonly TimeTrackerDbContext _dbContext;
    private readonly HttpContext _httpContext;
    private readonly ILogger<CreateRegularUserHandler> _logger;
    private readonly UserManager<User> _userManager;

    public CreateRegularUserHandler(ILogger<CreateRegularUserHandler> logger, TimeTrackerDbContext dbContext,
        UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<IServiceResultOf<CreateUserResponse>> Handle(CreateRegularUserRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var normalizedName = request.Name.ToUpperInvariant();
        if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == normalizedName || u.NormalizedEmail == normalizedEmail,
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

        await _dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResultOf<CreateUserResponse>.Success(
            new CreateUserResponse(newUser.Id, newUser.UserName, newUser.Email, new[] { "Regular" }),
            ResultTypeCode.Ok);
    }
}
