using Microsoft.AspNetCore.Mvc;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.Request;
using SL.Application.Models.ViewModels.EmailTemplate;

namespace SL.Web.Areas.Admin.Controller;

public class EmailTemplateController : BaseAdminController
{

    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ITokenizerService _tokenizerService;

    public EmailTemplateController(IEmailTemplateService emailTemplateService, ITokenizerService tokenizerService)
    {
        _emailTemplateService = emailTemplateService;
        _tokenizerService = tokenizerService;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _emailTemplateService.GetTemplateForEditAsync(id);

        if (result.IsFailure)
            return NotFound();

        return View(result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmailTemplateEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var freshDataResult = await _emailTemplateService.GetTemplateForEditAsync(viewModel.Id);
            if (freshDataResult.IsSuccess)
            {
                viewModel.AvailableEmailAccounts = freshDataResult.Value.AvailableEmailAccounts;
            }
            return View(viewModel);
        }

        var result = await _emailTemplateService.UpdateTemplateAsync(viewModel);

        if (result.IsSuccess)
            return RedirectToAction("List");

        ModelState.AddModelError(string.Empty, result.ErrorMessage);

        var freshDataResultOnFail = await _emailTemplateService.GetTemplateForEditAsync(viewModel.Id);
        if (freshDataResultOnFail.IsSuccess)
        {
            viewModel.AvailableEmailAccounts = freshDataResultOnFail.Value.AvailableEmailAccounts;
        }
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult List()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetData(DataTablesRequest dataTableRequest)
    {
        var response = await _emailTemplateService.GetEmailTemplatesForDataTablesAsync(dataTableRequest);
        return Ok(response);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleIsActive(Guid id)
    {
        var result = await _emailTemplateService.ToggleIsActiveAsync(id);

        if (result.IsSuccess)
            return Json(new { success = true, message = "Şablon durumu başarıyla değiştirildi." });

        return Json(new { success = false, message = result.ErrorMessage });
    }

    [HttpGet]
    public IActionResult DiscoverAllTokens()
    {
        var tokens = _tokenizerService.GetAllAllowedTokens();
        return Ok(tokens);
    }
}
