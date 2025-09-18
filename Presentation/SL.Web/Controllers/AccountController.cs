using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Framework;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Application.Interfaces.Services;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;
using SL.Domain.Entities;
using SL.Persistence.Contexts;

namespace SL.Web.Controllers;

public class AccountController : BasePublicController
{

    private readonly IAuthService _authService;
    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly IUnitOfWork<TenantDbContext> _unitOfWork;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(IAuthService authService, ITenantDatabaseService tenantDatabaseService,IUnitOfWork<TenantDbContext> unitOfWork)
    {
        _authService = authService;
        _tenantDatabaseService = tenantDatabaseService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        string tenantDatabaseName = $"SL_{Guid.NewGuid():N}";

        await _authService.RegisterAsync(registerViewModel, tenantDatabaseName);

        await _tenantDatabaseService.CreateDatabaseAsync(tenantDatabaseName);

        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home", new { area = AreaNames.CUSTOMER}));

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
    {
        returnUrl ??= Url.Content($"~/{AreaNames.CUSTOMER}");

        Result<string> result = await _authService.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe);
        if (result.IsSuccess)
        {
            _unitOfWork.ChangeDatabase($"Host=localhost;Port=5432;Database={result.Value};Username=postgres;Password=postgres");
            return LocalRedirect(returnUrl);
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(loginViewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity.IsAuthenticated)
            await _authService.LogoutAsync();

        return RedirectToAction("Login");
    }


}

