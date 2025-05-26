using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface ITOTPService
    {
        // Generate a new TOTP secret for a user
        Task<TOTPSecretDto> GenerateSecretAsync(string phoneNumber);

        Task<TOTPSecretDto> GetSecretForPhoneAsync(string phoneNumber);
        
        // Verify a TOTP code
        Task<bool> VerifyTOTPAsync(string phoneNumber, string totpCode);
        
        // Verify HOTP for sensitive operations
        Task<bool> VerifyHOTPAsync(string phoneNumber, string hotpCode);
        
        // Verify an SMS with OTP code
        Task<SMSVerificationResultDto> VerifySmsCommandAsync(SMSVerificationDto verification);
        
        // Reset or regenerate a secret
        Task<TOTPSecretDto> ResetSecretAsync(string phoneNumber, string idNumber);
    }
}