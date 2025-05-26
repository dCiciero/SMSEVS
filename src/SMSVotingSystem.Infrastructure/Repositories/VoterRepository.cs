using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        private readonly ApplicationDbContext _context;

        public VoterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Voter?> GetByIdAsync(int id)
        {
            return await _context.Voters.FindAsync(id);
        }

        public async Task<Voter?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var voter = await _context.Voters
                .FirstOrDefaultAsync(v => v.PhoneNumber == phoneNumber);
            if (voter == null)
                return null;  //throw new InvalidOperationException($"Voter with phone number '{phoneNumber}' not found.");
            return voter;
        }

        public async Task<IEnumerable<Voter>> GetAllAsync()
        {
            return await _context.Voters.ToListAsync();
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Voters
                .AnyAsync(v => v.PhoneNumber == phoneNumber);
        }

        public async Task AddAsync(Voter voter)
        {
            await _context.Voters.AddAsync(voter);
        }

        public Task UpdateAsync(Voter voter)
        {
            _context.Entry(voter).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task<bool> HasVotedInElectionAsync(int voterId, int electionId)
        {
            return await _context.Votes
                .AnyAsync(v => v.VoterId == voterId && v.ElectionId == electionId);
        }

        public async Task<bool> IdNumberExistsAsync(string idNumber)
        {
            return await _context.Voters
                .AnyAsync(v => v.IdNumber == idNumber);
        }
    }
}