using Microsoft.AspNetCore.Mvc;
using SL.Application.Framework;
using SL.Application.Interfaces.Services.Membership;
using SL.Application.Models.ViewModels.Account;
using SL.Domain;

namespace SL.Web.Controllers;

public class AccountController : BasePublicController
{
    private readonly IAuthService _authService;
    private readonly IRegistrationWorkflowService _registrationWorkflowService;
    public AccountController(IAuthService authService, IRegistrationWorkflowService registrationWorkflowService)
    {
        _authService = authService;
        _registrationWorkflowService = registrationWorkflowService;
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
            return View(registerViewModel);
        try
        {
            await _registrationWorkflowService.ExecuteRegistrationAsync(registerViewModel);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Kayıt işlemi başarısız oldu. Lütfen tekrar deneyin.");
            return View(registerViewModel);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home", new { area = AreaNames.CUSTOMER }));
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
    {
        returnUrl ??= Url.Content($"~/{AreaNames.CUSTOMER}");
        Result result = await _authService.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe);
        if (result.IsSuccess)
            return LocalRedirect(returnUrl);
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
