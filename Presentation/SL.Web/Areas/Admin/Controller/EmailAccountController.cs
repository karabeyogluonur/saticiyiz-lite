using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SL.Application.Interfaces.Services;
using SL.Application.Models.Request;
using SL.Application.Models.ViewModels.EmailAccount;
using SL.Domain;
using SL.Domain.Entities;
namespace SL.Web.Areas.Admin.Controller;

public class EmailAccountController : BaseAdminController
{
    private readonly IEmailAccountService _emailAccountService;
    private readonly IMapper _mapper;
    public EmailAccountController(IEmailAccountService emailAccountService, IMapper mapper)
    {
        _emailAccountService = emailAccountService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetData()
    {
        var request = new DataTablesRequest
        {
            Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault()),
            Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault()),
            Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault()),
            SearchValue = Request.Form["search[value]"].FirstOrDefault(),
            SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),
            SortDirection = Request.Form["order[0][dir]"].FirstOrDefault()
        };
        var response = await _emailAccountService.GetEmailAccountsForDataTablesAsync(request);
        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> Add(EmailAccountAddViewModel emailAccountCreateViewModel)
    {
        if (!ModelState.IsValid)
            return View(emailAccountCreateViewModel);

        var result = await _emailAccountService.CreateEmailAccountAsync(emailAccountCreateViewModel);

        if (result.IsSuccess)
            return RedirectToAction(nameof(List));
        else
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(emailAccountCreateViewModel);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _emailAccountService.GetEmailAccountForEditAsync(id);
        if (result.IsFailure) return NotFound();
        return View(result.Value);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmailAccountEditViewModel emailAccountEditViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(emailAccountEditViewModel);
        }
        Result result = await _emailAccountService.UpdateEmailAccountAsync(emailAccountEditViewModel);
        if (result.IsSuccess)
            return RedirectToAction(nameof(List));
        else
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(emailAccountEditViewModel);
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        Result result = await _emailAccountService.DeleteEmailAccountAsync(id);
        if (result.IsSuccess)
            return Json(new { success = true, message = "Hesap başarıyla silindi." });
        else
            return Json(new { success = false, message = result.ErrorMessage });
    }
}
