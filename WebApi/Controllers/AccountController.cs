using System.Threading.Tasks;
using Core.Model.ResponseModel;
using Core.Services;
using Core.ViewModel;
using Core.ViewModel.Account;
using Infrastructure.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[EnableCors("AllowSpecificOrigin")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<Response<TokenResponse>> Login(LoginRequest login)
    {
       return await _accountService.Login(login);   
    }

    [HttpPut]
    [Authorize(Permissions.Accounts.UpdateRoleClaims)]
    public async Task<Response<string>> UpdateRoleClaims(RoleClaimsUpdateRequest request)
    {
        return await _accountService.UpdateRoleClaims(request);
    }
}