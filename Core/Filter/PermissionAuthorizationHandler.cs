using Core.Services;  
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;  
using System.Security.Claims;
using Core.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace Core.Filter;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{ 
    private readonly CacheService _entityService;

    public PermissionAuthorizationHandler(CacheService entityService)
    { 
        _entityService = entityService;
    }
    

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Claims.IsNullOrEmpty()) return; 
        var listRoles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => new IdentityRole<Guid>(){Name = x.Value,NormalizedName = x.Value.ToUpper()}).ToList();
        var listPermissions = new List<PermissionListResponse>();
        foreach (var role in listRoles)
        {
            var permission = await _entityService.GetPermissions(role);
            listPermissions.AddRange(permission.Data);
        }
        
        var canAccess = listPermissions.Any(c => c.Type == "Permission" && c.Value == requirement.Permission);

        if (canAccess)
        {
            context.Succeed(requirement);
        }

    }
}