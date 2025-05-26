using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface IVoterService
    {
        Task<IEnumerable<VoterDto>> GetAllVotersAsync();
        Task<VoterDto> GetVoterByIdAsync(int id);
        Task<VoterDto> GetVoterByPhoneNumberAsync(string phoneNumber);
        Task<VoterDto> RegisterVoterAsync(CreateVoterDto createVoterDto);
        Task<VoterDto> UpdateVoterAsync(int id, UpdateVoterDto updateVoterDto);
        Task DeleteVoterAsync(int id);
    }
}