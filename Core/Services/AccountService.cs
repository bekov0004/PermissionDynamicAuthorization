using System.IdentityModel.Tokens.Jwt;
using Core.Model.ResponseModel;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;
using System.Text;
using Core.ViewModel;
using Core.ViewModel.Account;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;  

namespace Core.Services;
public class AccountService
{
     
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser<Guid>> _userManager;  
    private readonly DataContext _dataContext;
    private readonly CacheService _cacheService;


    public AccountService(IConfiguration configuration,
        UserManager<IdentityUser<Guid>> userManager,
        DataContext dataContext,CacheService cacheService)
    { 
        _configuration = configuration;
        _userManager = userManager; 
        _dataContext = dataContext;
        _cacheService = cacheService;
    }



    public async Task<Response<TokenResponse>> Login(LoginRequest login)
    {

        var user = await _userManager.FindByNameAsync(login.UserName);

        if (user == null) return new Response<TokenResponse>(HttpStatusCode.BadRequest, new List<string>() { "Вы не регистрировани" });

        var check = await _userManager.CheckPasswordAsync(user, login.Password);
        if (!user.EmailConfirmed)
        {
            return new Response<TokenResponse>(HttpStatusCode.BadRequest, new List<string>() { "Пожалуйста подвердите электронную почу чтобы войти в систему" });
        }

        if (check) 
        { 
            return new Response<TokenResponse>(await GenerateJwtToken(user));
        }

        return new Response<TokenResponse>(HttpStatusCode.BadRequest, new List<string>() { "Неверный парол" });


    }


    private async Task<TokenResponse> GenerateJwtToken(IdentityUser<Guid> user)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

        //get all roles belonging to the user
        var roles = await _userManager.GetRolesAsync(user); //add all roles into claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //fill token 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return new TokenResponse(tokenString);
    }


    public async Task<Response<string>> UpdateRoleClaims(RoleClaimsUpdateRequest request)
    {
        var existingRoleClaims = await _dataContext.RoleClaims.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingRoleClaims == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, new List<string>() { "Доступ не найден" });
        }

        existingRoleClaims.ClaimType = request.CliamsType;
        existingRoleClaims.ClaimValue = request.ClaimsValue;
        await _dataContext.SaveChangesAsync();
        var existingRole =await _dataContext.Roles.FirstOrDefaultAsync(x => x.Id == existingRoleClaims.RoleId);
        if (existingRole == null) 
            return new Response<string>(HttpStatusCode.NotFound,new List<string>(){"Рол не найден"});

        await _cacheService.UpdateCache(existingRole);
        return new Response<string>("Все ок");
    }
}