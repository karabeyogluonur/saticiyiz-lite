using Microsoft.AspNetCore.Mvc.Rendering;
using SL.Application.Extensions;
using SL.Application.Interfaces.Factories.Messages;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.ViewModels.QueuedEmail;
using SL.Domain.Enums.Messages;



namespace SL.Web.Factories
{
    public class QueuedEmailModelFactory : IQueuedEmailModelFactory
    {
        private readonly IEmailAccountService _emailAccountService;

        public QueuedEmailModelFactory(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        public async Task<QueuedEmailListViewModel> PrepareListViewModelAsync()
        {
            var accounts = await _emailAccountService.GetAllAsync();
            var statuses = Enum.GetValues(typeof(QueuedEmailStatus))
                               .Cast<QueuedEmailStatus>()
                               .Select(e => new SelectListItem
                               {
                                   Value = ((int)e).ToString(),
                                   Text = e.GetDisplayName()
                               });

            var viewModel = new QueuedEmailListViewModel
            {
                AvailableEmailAccounts = new SelectList(accounts, "Id", "DisplayName"),
                AvailableStatuses = statuses
            };

            return viewModel;
        }
    }
}