using Microsoft.AspNetCore.Identity;
using NSubstitute;
using RGamaFelix.TimeTracker.Domain.Model;

namespace RGamaFelix.TimeTracker.Tests;

public class TestHelper
{
    public static UserManager<User> MockUserManager()
    {
        var storeMock = Substitute.For<IUserStore<User>>();
        var userManagerMock =
            Substitute.For<UserManager<User>>(storeMock, null, null, null, null, null, null, null, null);
        return userManagerMock;
    }
}