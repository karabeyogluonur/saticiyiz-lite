

using SL.Application.Models.ViewModels.QueuedEmail;

namespace SL.Application.Interfaces.Factories.Messages
{
    public interface IQueuedEmailModelFactory
    {
        Task<QueuedEmailListViewModel> PrepareListViewModelAsync();
    }
}