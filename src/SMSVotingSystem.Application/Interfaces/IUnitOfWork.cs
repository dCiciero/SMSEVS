using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.Interfaces
{
     public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}