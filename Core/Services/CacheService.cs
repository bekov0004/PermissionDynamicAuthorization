using System.Net; 
using Core.ViewModel; 
using Infrastructure.Data; 
using Core.Model.ResponseModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; 
using Microsoft.Extensions.Caching.Memory;

namespace Core.Services;

public class CacheService
{ 
    private readonly DataContext _dataDataContext;
    private readonly IMemoryCache _memoryCache; 
    public CacheService(DataContext dataDataContext, IMemoryCache memoryCache)
    { 
        _dataDataContext = dataDataContext;  
        _memoryCache = memoryCache;
    }

    public async Task<Response<string>> SetCache()
    {
        var listRoles = await _dataDataContext.Roles.Select(x => x).ToListAsync();
        if (listRoles.IsNullOrEmpty()) return new Response<string>("Роли не найдены");
        foreach (var role in listRoles)
        {
            var key = string.Concat("Permission", "-", role.Name);
            var listPermissions =  await _dataDataContext.RoleClaims.Where(x => x.RoleId == role.Id).Select(x => new PermissionListResponse(x.ClaimType, x.ClaimValue)).ToListAsync();
            _memoryCache.Set(key, listPermissions);
        }
        return new Response<string>("ОК");
    }
    public async Task<Response<List<PermissionListResponse>>> GetPermissions(IdentityRole<Guid> role)
    {
        var existingRole = await _dataDataContext.Roles.FirstOrDefaultAsync(x => x.Name == role.Name);
        if (existingRole == null) return new Response<List<PermissionListResponse>>(HttpStatusCode.NotFound, new List<string>() { "Роль не найден" });
       
        var key = string.Concat("Permission", "-", existingRole.Name);
        var listPermissions = _memoryCache.Get<List<PermissionListResponse>>(key);  

        if (listPermissions == null || listPermissions.Count == 0)
        {
            listPermissions =  await _dataDataContext.RoleClaims.Where(x => x.RoleId == existingRole.Id).Select(x => new PermissionListResponse(x.ClaimType, x.ClaimValue)).ToListAsync();
            _memoryCache.Set(key, listPermissions);
        }


        return new Response<List<PermissionListResponse>>(listPermissions);
    }

    public async Task<Response<string>> UpdateCache(IdentityRole<Guid> role)
    {
        var existingRole = await _dataDataContext.Roles.FirstOrDefaultAsync(x => x.Name == role.Name);
        if (existingRole == null) return new Response<string>(HttpStatusCode.NotFound, new List<string>() { "Роль не найден" });
        
        var key = string.Concat("Permission", "-", existingRole.Name);
        _memoryCache.Remove(key);
        var listPermissions = await _dataDataContext.RoleClaims.Where(x => x.RoleId == existingRole.Id).Select(x => new PermissionListResponse(x.ClaimType, x.ClaimValue)).ToListAsync();
        if (listPermissions.IsNullOrEmpty())
        {
            return new Response<string>(HttpStatusCode.NotFound, new List<string>() { "Разрешение не найден" });
        }

        _memoryCache.Set(key, listPermissions);

        return new Response<string>("OK");
    }

}