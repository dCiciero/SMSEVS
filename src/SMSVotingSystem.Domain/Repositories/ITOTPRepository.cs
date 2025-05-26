using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Domain.Repositories
{
    public interface ITOTPRepository
    {
        Task<TOTPSecret> GetByPhoneNumberAsync(string phoneNumber);
        Task<TOTPSecret> CreateAsync(TOTPSecret secret);
        Task UpdateAsync(TOTPSecret secret);
        Task<bool> RecordFailedAttemptAsync(string phoneNumber);
        Task<bool> ResetFailedAttemptsAsync(string phoneNumber);
    }
}