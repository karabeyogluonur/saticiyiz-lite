using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SL.Application.Interfaces.Services;
using SL.Application.Models.ViewModels.Account;
using SL.Domain.Entities;
using SL.Persistence.Contexts;

namespace SL.Web.Controllers;

public class AccountController : BasePublicController
{

    private readonly IAuthService _authService;
    private readonly ITenantDatabaseService _tenantDatabaseService;

    public AccountController(IAuthService authService, ITenantDatabaseService tenantDatabaseService)
    {
        _authService = authService;
        _tenantDatabaseService = tenantDatabaseService;
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
}

