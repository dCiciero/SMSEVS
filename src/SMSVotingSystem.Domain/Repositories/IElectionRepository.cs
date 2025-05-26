using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface IElectionRepository
    {
        Task<Election> GetByIdAsync(int id);
        Task<IEnumerable<Election>> GetAllAsync();
        Task<Election?> GetActiveElectionAsync();
        Task AddAsync(Election election);
        Task UpdateAsync(Election election);
        Task DeleteAsync(int id);
    }
}