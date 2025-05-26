using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface IVoterRepository
    {
        Task<Voter?> GetByIdAsync(int id);
        Task<Voter?> GetByPhoneNumberAsync(string phoneNumber);
        Task<IEnumerable<Voter>> GetAllAsync();
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task AddAsync(Voter voter);
        Task UpdateAsync(Voter voter);
        Task<bool> HasVotedInElectionAsync(int voterId, int electionId);

        //check for iDNumber uniqueness
        Task<bool> IdNumberExistsAsync(string idNumber);
    }
}