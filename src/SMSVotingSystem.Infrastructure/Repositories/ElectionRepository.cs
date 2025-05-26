using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class ElectionRepository : IElectionRepository
    {
        private readonly ApplicationDbContext _context;
        
        public ElectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Election> GetByIdAsync(int id)
        {
            return await _context.Elections.FindAsync(id)
                ?? throw new KeyNotFoundException($"Election with ID {id} not found.");
        }
        
        public async Task<IEnumerable<Election>> GetAllAsync()
        {
            return await _context.Elections.ToListAsync();
        }
        
        public async Task<Election?> GetActiveElectionAsync()
        {
            return await _context.Elections
                .FirstOrDefaultAsync(e => e.IsActive); 
                //?? throw new InvalidOperationException("No active election found. 222");
        }
        
        public async Task AddAsync(Election election)
        {
            await _context.Elections.AddAsync(election);
        }
        
        public Task UpdateAsync(Election election)
        {
            _context.Entry(election).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        
        public async Task DeleteAsync(int id)
        {
            var election = await _context.Elections.FindAsync(id);
            if (election != null)
            {
                _context.Elections.Remove(election);
            }
        }
    }
}