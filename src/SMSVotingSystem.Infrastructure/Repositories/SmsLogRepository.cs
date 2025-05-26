using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class SmsLogRepository : ISmsLogRepository
    {
        private readonly ApplicationDbContext _context;
        
        public SmsLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<SmsLog> GetByIdAsync(int id)
        {
            return await _context.SmsLogs.FindAsync(id);
        }
        
        public async Task<IEnumerable<SmsLog>> GetAllAsync()
        {
            return await _context.SmsLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<SmsLog>> GetRecentLogsAsync(int count)
        {
            return await _context.SmsLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();
        }
        
        public async Task AddAsync(SmsLog log)
        {
            await _context.SmsLogs.AddAsync(log);
        }
        
        public Task UpdateAsync(SmsLog log)
        {
            _context.Entry(log).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}