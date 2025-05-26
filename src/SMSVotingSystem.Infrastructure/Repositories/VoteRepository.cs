using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;
        
        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Vote> GetByIdAsync(int id)
        {
            return await _context.Votes.FindAsync(id);
        }
        
        public async Task<IEnumerable<Vote>> GetByElectionIdAsync(int electionId)
        {
            return await _context.Votes
                .Include(v => v.Candidate)
                .Where(v => v.ElectionId == electionId)
                .ToListAsync();
        }
        
        public async Task<int> GetVoteCountForElectionAsync(int electionId)
        {
            return await _context.Votes.CountAsync(v => v.ElectionId == electionId);
        }
        
        public async Task<bool> HasVotedInElectionAsync(int voterId, int electionId)
        {
            return await _context.Votes
                .AnyAsync(v => v.VoterId == voterId && v.ElectionId == electionId);
        }
        
        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
        }
    }
}