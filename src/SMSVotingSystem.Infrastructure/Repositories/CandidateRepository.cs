using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicationDbContext _context;
        
        public CandidateRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Candidate> GetByIdAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            return candidate ?? throw new InvalidOperationException("Candidate with the specified ID not found.");
        }
        
        public async Task<Candidate?> GetByShortCodeAsync(string shortCode)
        {
            return await _context.Candidates
                .FirstOrDefaultAsync(c => c.ShortCode == shortCode);
                
        }
        
        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            return await _context.Candidates.ToListAsync();
        }
        
        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _context.Candidates
                .AnyAsync(c => c.ShortCode == shortCode);
        }
        
        public async Task AddAsync(Candidate candidate)
        {
            await _context.Candidates.AddAsync(candidate);
        }
        
        public Task UpdateAsync(Candidate candidate)
        {
            _context.Entry(candidate).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        
        public async Task DeleteAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
            }
        }

        public async Task<IEnumerable<Candidate>> GetByElectionIdAsync(int electionId)
        {
            return await _context.Candidates
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();
        }
    }
}