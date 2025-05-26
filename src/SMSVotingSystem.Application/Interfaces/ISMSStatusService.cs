using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface ISMSStatusService
    {
        Task<SMSResponseDto> ProcessStatusSmsAsync(SMSVerificationDto command);
    }
}