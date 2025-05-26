using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface ISmsLogRepository
    {
        Task<SmsLog> GetByIdAsync(int id);
        Task<IEnumerable<SmsLog>> GetAllAsync();
        Task<IEnumerable<SmsLog>> GetRecentLogsAsync(int count);
        Task AddAsync(SmsLog log);
        Task UpdateAsync(SmsLog log);
    }
}