using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface ISMSHelpService
    {
        Task<SMSResponseDto> ProcessHelpSmsAsync(SMSVerificationDto command);
    }
}