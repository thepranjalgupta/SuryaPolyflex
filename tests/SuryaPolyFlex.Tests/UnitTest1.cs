using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Moq;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;
using SuryaPolyFlex.Infrastructure.Services;

namespace SuryaPolyFlex.Tests;

public class UnitTest1
{
    [Fact]
    public async Task CreateRuleAsync_InvalidPermission_ThrowsInvalidOperationException()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new UserManager<ApplicationUser>(
            userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var ruleServiceMock = new Mock<IRuleEvaluationService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var permissionService = new PermissionService(context, userManager, ruleServiceMock.Object, memoryCache);

        var rule = new Rule
        {
            Name = "InvalidRule",
            Description = "Invalid permission id rule",
            PermissionId = 999,
            Scope = RuleScope.Owner,
            IsActive = true,
            CreatedBy = "test"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await permissionService.CreateRuleAsync(rule));
    }
}

