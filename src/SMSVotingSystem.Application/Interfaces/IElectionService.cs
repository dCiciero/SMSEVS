using System.Collections.Generic;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface IElectionService
    {
        /// <summary>
        /// Get all elections in the system
        /// </summary>
        Task<IEnumerable<ElectionDto>> GetAllElectionsAsync();

        /// <summary>
        /// Get an election by its ID
        /// </summary>
        Task<ElectionDto> GetElectionByIdAsync(int id);

        /// <summary>
        /// Get the currently active election
        /// </summary>
        Task<ElectionDto> GetActiveElectionAsync();

        /// <summary>
        /// Create a new election
        /// </summary>
        Task<ElectionDto> CreateElectionAsync(CreateElectionDto createElectionDto);

        /// <summary>
        /// Update an existing election
        /// </summary>
        Task<ElectionDto> UpdateElectionAsync(int id, UpdateElectionDto updateElectionDto);

        /// <summary>
        /// Activate an election (will deactivate any currently active election)
        /// </summary>
        Task<ElectionDto> ActivateElectionAsync(int id);

        /// <summary>
        /// Deactivate an election
        /// </summary>
        Task<ElectionDto> DeactivateElectionAsync(int id);

        /// <summary>
        /// Delete an election
        /// </summary>
        Task DeleteElectionAsync(int id);

        
        Task<IEnumerable<CandidateDto>> GetCandidatesForElectionAsync(int electionId);
    }
}