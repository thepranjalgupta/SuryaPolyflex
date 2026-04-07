using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SuryaPolyFlex.Application.Common.Interfaces;

namespace SuryaPolyFlex.Web.Filters;

public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(string permission)
        : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }
}

public class PermissionFilter : IAsyncAuthorizationFilter
{
    private readonly IPermissionService _permissionService;
    private readonly string _permission;

    public PermissionFilter(IPermissionService permissionService, string permission)
    {
        _permissionService = permissionService;
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var userId = user.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasPermission = await _permissionService.HasPermissionAsync(userId, _permission, null);
        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
        }
    }
}