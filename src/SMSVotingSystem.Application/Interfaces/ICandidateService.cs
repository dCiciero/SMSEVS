using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface ICandidateService
    {
        Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync();
        Task<CandidateDto> GetCandidateByIdAsync(int id);
        Task<CandidateDto> GetCandidateByShortCodeAsync(string shortCode);
        Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto);
        Task<CandidateDto> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto);
        Task DeleteCandidateAsync(int id);
    }
}