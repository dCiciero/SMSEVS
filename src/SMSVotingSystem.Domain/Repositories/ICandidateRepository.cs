using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface ICandidateRepository
    {
        Task<Candidate> GetByIdAsync(int id);
        Task<Candidate> GetByShortCodeAsync(string shortCode);
        Task<IEnumerable<Candidate>> GetAllAsync();
        Task<bool> ShortCodeExistsAsync(string shortCode);
        Task AddAsync(Candidate candidate);
        Task UpdateAsync(Candidate candidate);
        Task DeleteAsync(int id);
        Task<IEnumerable<Candidate>> GetByElectionIdAsync(int electionId);
    }
}