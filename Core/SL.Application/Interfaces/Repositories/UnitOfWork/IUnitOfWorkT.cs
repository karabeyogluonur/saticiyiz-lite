using System;
using Microsoft.EntityFrameworkCore;
namespace SL.Application.Interfaces.Repositories.UnitOfWork
{
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext DbContext { get; }
        Task<int> SaveChangesAsync(bool ensureAutoHistory = false, params IUnitOfWork[] unitOfWorks);
    }
}
