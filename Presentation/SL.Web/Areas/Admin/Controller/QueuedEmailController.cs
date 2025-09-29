using Microsoft.AspNetCore.Mvc;
using SL.Application.Interfaces.Factories; // Yeni using
using SL.Application.Interfaces.Factories.Messages;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.Request;
using System;
using System.Threading.Tasks;

namespace SL.Web.Areas.Admin.Controller
{
    public class QueuedEmailController : BaseAdminController
    {
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IQueuedEmailModelFactory _queuedEmailModelFactory; // Yeni Factory

        public QueuedEmailController(
            IQueuedEmailService queuedEmailService,
            IQueuedEmailModelFactory queuedEmailModelFactory)
        {
            _queuedEmailService = queuedEmailService;
            _queuedEmailModelFactory = queuedEmailModelFactory;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var viewModel = await _queuedEmailModelFactory.PrepareListViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetData(DataTablesRequest dataTableRequest)
        {
            var response = await _queuedEmailService.GetQueuedEmailsForDataTablesAsync(dataTableRequest);
            return Ok(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _queuedEmailService.DeleteQueuedEmailAsync(id);
            if (result.IsSuccess)
            {
                return Json(new { success = true, message = "E-posta kuyruktan başarıyla silindi." });
            }
            return Json(new { success = false, message = result.ErrorMessage });
        }
    }
}