using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface IVoteRepository
    {
        Task<Vote> GetByIdAsync(int id);
        Task<IEnumerable<Vote>> GetByElectionIdAsync(int electionId);
        Task<int> GetVoteCountForElectionAsync(int electionId);
        Task<bool> HasVotedInElectionAsync(int voterId, int electionId);
        Task AddAsync(Vote vote);
    }
}