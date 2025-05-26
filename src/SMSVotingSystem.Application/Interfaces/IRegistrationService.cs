using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface IRegistrationService
    {
        /// <summary>
        /// Process a voter registration request received via SMS
        /// </summary>
        /// <param name="phoneNumber">The phone number of the potential voter</param>
        /// <param name="message">The SMS message content containing the registration request</param>
        /// <returns>True if the registration was processed successfully, false otherwise</returns>
        Task<SMSResponseDto> ProcessRegistrationRequestAsync(string phoneNumber, string message);
    }
}