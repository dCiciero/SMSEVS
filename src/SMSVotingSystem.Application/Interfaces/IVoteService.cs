using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface IVoteService
    {
        // <summary>
        /// Process a vote received via SMS
        /// </summary>
        /// <param name="phoneNumber">The phone number of the voter</param>
        /// <param name="message">The SMS message content containing the vote</param>
        /// <returns>True if the vote was processed successfully, false otherwise</returns>
        Task<SMSResponseDto> ProcessVoteBySmsAsync(string phoneNumber, string message);
        
        /// <summary>
        /// Get the voting results for a specific election
        /// </summary>
        /// <param name="electionId">The ID of the election</param>
        /// <returns>A collection of vote results</returns>
        Task<IEnumerable<VoteResultDto>> GetVoteResultsAsync(int electionId);
        Task<bool> HasVotedAsync(int voterId, int electionId);
    }
}