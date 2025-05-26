using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Repositories;
using Microsoft.Extensions.Options;
using SMSVotingSystem.Application.Common;

using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Services
{
    public class TOTPService : ITOTPService
    {
        private readonly ITOTPRepository _totpRepository;
        private readonly IVoterRepository _voterRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<TOTPService> _logger;
        private readonly SecuritySettings _settings;

        public TOTPService(
            ITOTPRepository totpRepository,
            IVoterRepository voterRepository,
            IEncryptionService encryptionService,
            ILogger<TOTPService> logger,
            IOptions<SecuritySettings> settings)
        {
            _totpRepository = totpRepository;
            _voterRepository = voterRepository;
            _encryptionService = encryptionService;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<TOTPSecretDto> GenerateSecretAsync(string phoneNumber)
        {
            // Check if a secret already exists
            var existingSecret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existingSecret != null && existingSecret.IsActive)
            {
                throw new InvalidOperationException("Active secret already exists for this phone number");
            }

            // Generate new secret
            string plainSecret = TOTPUtility.GenerateRandomSecret();
            string encryptedSecret = _encryptionService.Encrypt(plainSecret);

            var secret = new TOTPSecret
            {
                PhoneNumber = phoneNumber,
                EncryptedSecretKey = encryptedSecret,
                CreatedAt = DateTime.UtcNow,
                Counter = 0,
                IsActive = true,
                FailedAttempts = 0
            };

            await _totpRepository.CreateAsync(secret);



            // Return DTO with plain secret (only time it's returned unencrypted)
            return new TOTPSecretDto
            {
                PhoneNumber = phoneNumber,
                SecretKey = plainSecret,
                CreatedAt = secret.CreatedAt,
                Counter = 0,
                IsActive = true,
                // CurrentCode = TOTPUtility.GenerateCurrentCode(plainSecret, _settings.TOTPDigits, _settings.TOTPTimeStepSeconds),
            };
        }

        public async Task<TOTPSecretDto> GetSecretForPhoneAsync(string phoneNumber)
        {
            try
            {
                var secret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);

                if (secret == null)
                {
                    _logger.LogWarning("TOTP secret not found for phone {PhoneNumber}", phoneNumber);
                    return null;
                }

                // Check if the secret is locked out
                if (secret.LockoutEnd.HasValue && secret.LockoutEnd > DateTime.UtcNow)
                {
                    _logger.LogWarning("TOTP secret for {PhoneNumber} is locked out until {LockoutEnd}",
                        phoneNumber, secret.LockoutEnd);

                    return new TOTPSecretDto
                    {
                        PhoneNumber = secret.PhoneNumber,
                        SecretKey = null, // Don't return the actual key for locked accounts
                        CreatedAt = secret.CreatedAt,
                        IsActive = secret.IsActive,
                        IsLocked = true,
                        LockoutEnd = secret.LockoutEnd
                    };
                }

                // Decrypt the secret
                string decryptedSecret = _encryptionService.Decrypt(secret.EncryptedSecretKey);

                return new TOTPSecretDto
                {
                    PhoneNumber = secret.PhoneNumber,
                    SecretKey = decryptedSecret,
                    CreatedAt = secret.CreatedAt,
                    IsActive = secret.IsActive,
                    IsLocked = false,
                    LockoutEnd = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TOTP secret for {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        public async Task<bool> VerifyTOTPAsync(string phoneNumber, string totpCode)
        {
            var secret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);
            if (secret == null || !secret.IsActive)
            {
                _logger.LogWarning("TOTP verification failed: No active secret for {PhoneNumber}", phoneNumber);
                return false;
            }

            // Check for lockout
            if (secret.LockoutEnd.HasValue && secret.LockoutEnd.Value > DateTime.UtcNow)
            {
                _logger.LogWarning("TOTP verification attempted for locked account: {PhoneNumber}", phoneNumber);
                return false;
            }

            // Decrypt secret
            string plainSecret = _encryptionService.Decrypt(secret.EncryptedSecretKey);

            // Verify TOTP
            bool isValid = TOTPUtility.VerifyTOTP(
                plainSecret,
                totpCode,
                _settings.TOTPDigits,
                _settings.TOTPTimeStepSeconds,
                _settings.TOTPWindowSize);

            if (isValid)
            {
                // Reset failed attempts on success
                secret.FailedAttempts = 0;
                secret.LastUsed = DateTime.UtcNow;
                await _totpRepository.ResetFailedAttemptsAsync(phoneNumber);
            }
            else
            {
                // Increment failed attempts
                await _totpRepository.RecordFailedAttemptAsync(phoneNumber);
            }

            return isValid;
        }

        public async Task<bool> VerifyHOTPAsync(string phoneNumber, string hotpCode)
        {
            var secret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);
            if (secret == null || !secret.IsActive)
            {
                return false;
            }

            // Check for lockout
            if (secret.LockoutEnd.HasValue && secret.LockoutEnd.Value > DateTime.UtcNow)
            {
                return false;
            }

            // Decrypt secret
            string plainSecret = _encryptionService.Decrypt(secret.EncryptedSecretKey);

            // Verify HOTP
            bool isValid = TOTPUtility.VerifyHOTP(
                plainSecret,
                hotpCode,
                secret.Counter,
                _settings.TOTPDigits,
                _settings.HOTPLookAheadWindow);

            if (isValid)
            {
                // Update counter and reset failed attempts
                secret.Counter++;
                secret.FailedAttempts = 0;
                secret.LastUsed = DateTime.UtcNow;
                await _totpRepository.UpdateAsync(secret);
            }
            else
            {
                // Increment failed attempts
                await _totpRepository.RecordFailedAttemptAsync(phoneNumber);
            }

            return isValid;
        }


        public async Task<SMSVerificationResultDto> VerifySmsCommandAsync(SMSVerificationDto verification)
        {
            try
            {
                // Parse the SMS message
                string[] parts = verification.Message.Trim().Split(' ');

                // Check format
                if (parts.Length < 1)
                {
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = "Invalid format. Text HELP for assistance."
                    };
                }

                string command = parts[0].ToUpper();

                // List of all valid commands
                string[] validCommands = { "REG", "REGISTER", "VOTE", "STATUS", "RESET", "HELP", "HOW", "CODE", "CANDIDATES",  "CAND" };
                if (!validCommands.Contains(command))
                {
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = "Unknown command. Text HELP for a list of valid commands."
                    };
                }

                

                // Before OTP verification, check if phone number exists
                var voter = await _voterRepository.GetByPhoneNumberAsync(verification.PhoneNumber);
                if (voter == null)
                {
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = "You are not registered. Text REG [ID number] [full name] to register."
                    };
                }

                // Check if account is active
                if (!voter.IsActive)
                {
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = "Your account is inactive. Please contact the election administrator."
                    };
                }


                // Commands that don't require OTP verification
                string[] exemptCommands = { "REG", "REGISTER", "HOW", "CODE", "HELP", "CANDIDATES", "CAND", "STATUS", "RESET" };  //, 
                if (exemptCommands.Contains(command))
                {
                    verification.Command = command;
                    verification.OTP = null;
                    verification.Parameters = parts.Length > 1 ? parts.Skip(1).ToArray() : Array.Empty<string>();

                    _logger.LogInformation("Processing exempt command {Command} from {PhoneNumber}", command, verification.PhoneNumber);

                    return new SMSVerificationResultDto { IsValid = true };
                }

                // Check if account is locked before attempting verification
                var totpInfo = await GetSecretForPhoneAsync(verification.PhoneNumber);
                if (totpInfo != null && totpInfo.LockoutEnd.HasValue && totpInfo.LockoutEnd.HasValue)
                {
                    TimeSpan lockoutRemaining = totpInfo.LockoutEnd.Value - DateTime.UtcNow;
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = $"Your account is temporarily locked due to too many failed attempts. Try again in {Math.Ceiling(lockoutRemaining.TotalMinutes)} minutes."
                    };
                }

                // For commands requiring OTP, check format
                if (parts.Length < 2)
                {
                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = $"Invalid format. Use: {command} [verification code] [parameters]"
                    };
                }

                string otp = parts[1];

                // Verify OTP
                bool isValidOTP = await VerifyTOTPAsync(verification.PhoneNumber, otp);

                if (!isValidOTP)
                {
                    _logger.LogWarning("Failed OTP verification attempt for phone {PhoneNumber}", verification.PhoneNumber);

                    // Increment failed attempts
                    await RecordFailedAttemptAsync(verification.PhoneNumber);

                    // Check if account is now locked
                    totpInfo = await GetSecretForPhoneAsync(verification.PhoneNumber);
                    if (totpInfo != null && totpInfo.LockoutEnd.HasValue && totpInfo.LockoutEnd.HasValue)
                    {
                        TimeSpan lockoutRemaining = totpInfo.LockoutEnd.Value - DateTime.UtcNow;
                        return new SMSVerificationResultDto
                        {
                            IsValid = false,
                            ResponseMessage = $"Too many failed attempts. Your account is now locked. Try again in {Math.Ceiling(lockoutRemaining.TotalMinutes)} minutes."
                        };
                    }

                    return new SMSVerificationResultDto
                    {
                        IsValid = false,
                        ResponseMessage = "Invalid verification code. Text CODE to get a new verification code."
                    };
                }

                // Reset failed attempts on successful verification
                await ResetFailedAttemptsAsync(verification.PhoneNumber);

                _logger.LogInformation("Successful OTP verification for {PhoneNumber}, command {Command}", verification.PhoneNumber, command);

                // Extract command parameters
                verification.Command = command;
                verification.OTP = otp;
                verification.Parameters = parts.Length > 2 ? parts.Skip(2).ToArray() : Array.Empty<string>();

                return new SMSVerificationResultDto
                {
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying SMS command for {PhoneNumber}: {Message}", verification.PhoneNumber, verification.Message);

                return new SMSVerificationResultDto
                {
                    IsValid = false,
                    ResponseMessage = "An error occurred while processing your request. Please try again later."
                };
            }
        }

        // public async Task<SMSVerificationResultDto> VerifySmsCommandAsync(SMSVerificationDto verification)
        // {
        //     // Parse the SMS message
        //     string[] parts = verification.Message.Trim().Split(' ');

        //     // Check format
        //     if (parts.Length < 2)
        //     {
        //         return new SMSVerificationResultDto
        //         {
        //             IsValid = false,
        //             ResponseMessage = "Invalid format. Use: COMMAND OTP [PARAMETERS]"
        //         };
        //     }

        //     string command = parts[0].ToUpper();
        //     string otp = parts[1];

        //     // Special case for registration
        //     if ((command == "REG" || command == "REGISTER") && parts.Length > 1) // && parts[1].ToUpper() == "NEW")
        //     {
        //         // New registration doesn't need OTP verification
        //         verification.Command = command;
        //         verification.OTP = null;
        //         verification.Parameters = parts.Skip(1).ToArray();

        //         return new SMSVerificationResultDto
        //         {
        //             IsValid = true
        //         };
        //     }

        //     if (command == "HOW" || command == "CODE")
        //     {
        //         // New registration doesn't need OTP verification
        //         verification.Command = command;
        //         verification.OTP = null;
        //         verification.Parameters = parts.Skip(1).ToArray();

        //         return new SMSVerificationResultDto
        //         {
        //             IsValid = true
        //         };
        //     }

        //     // Verify OTP
        //     bool isValidOTP = await VerifyTOTPAsync(verification.PhoneNumber, otp);

        //     if (!isValidOTP)
        //     {
        //         return new SMSVerificationResultDto
        //         {
        //             IsValid = false,
        //             ResponseMessage = "Invalid verification code. Please try again."
        //         };
        //     }

        //     // Extract command parameters
        //     verification.Command = command;
        //     verification.OTP = otp;
        //     verification.Parameters = parts.Length > 2 ? parts.Skip(2).ToArray() : Array.Empty<string>();

        //     return new SMSVerificationResultDto
        //     {
        //         IsValid = true
        //     };
        // }

        public async Task<TOTPSecretDto> ResetSecretAsync(string phoneNumber, string idNumber)
        {
            // Verify voter exists and ID matches
            var voter = await _voterRepository.GetByPhoneNumberAsync(phoneNumber);
            if (voter == null || voter.IdNumber != idNumber)
            {
                throw new UnauthorizedAccessException("Invalid phone number or ID");
            }

            // Deactivate any existing secrets
            var existingSecret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existingSecret != null)
            {
                existingSecret.IsActive = false;
                await _totpRepository.UpdateAsync(existingSecret);
            }

            // Generate new secret
            return await GenerateSecretAsync(phoneNumber);
        }

        /// <summary>
        /// Records a failed TOTP verification attempt for the specified phone number
        /// Increments the failed attempt counter and locks the account if maximum attempts are reached
        /// </summary>
        /// <param name="phoneNumber">The phone number to record the failed attempt for</param>
        /// <returns>The updated TOTP secret information</returns>
        public async Task<TOTPSecretDto> RecordFailedAttemptAsync(string phoneNumber)
        {
            try
            {
                var secret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);

                if (secret == null)
                {
                    _logger.LogWarning("Attempted to record failed attempt for non-existent phone {PhoneNumber}", phoneNumber);
                    return null;
                }

                // Increment failed attempts
                secret.FailedAttempts += 1;

                // Check if max attempts reached and account should be locked
                if (secret.FailedAttempts >= _settings.MaxFailedAttempts)
                {
                    // secret.IsLocked = true;
                    secret.LockoutEnd = DateTime.UtcNow.AddMinutes(_settings.LockoutMinutes);

                    _logger.LogWarning("Account for {PhoneNumber} locked until {LockoutEnd} due to {FailedAttempts} failed attempts",
                        phoneNumber, secret.LockoutEnd, secret.FailedAttempts);
                }

                // Update in database
                await _totpRepository.UpdateAsync(secret);

                // Return DTO
                return new TOTPSecretDto
                {
                    PhoneNumber = secret.PhoneNumber,
                    SecretKey = null, // Don't return the actual secret key
                    CreatedAt = secret.CreatedAt,
                    IsActive = secret.IsActive,
                    // IsLocked = secret.IsLocked,
                    LockoutEnd = secret.LockoutEnd,
                    FailedAttempts = secret.FailedAttempts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording failed attempt for {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        /// <summary>
        /// Resets the failed attempts counter after a successful verification
        /// </summary>
        /// <param name="phoneNumber">The phone number to reset the failed attempts for</param>
        /// <returns>The updated TOTP secret information</returns>
        public async Task<TOTPSecretDto> ResetFailedAttemptsAsync(string phoneNumber)
        {
            try
            {
                var secret = await _totpRepository.GetByPhoneNumberAsync(phoneNumber);

                if (secret == null)
                {
                    _logger.LogWarning("Attempted to reset failed attempts for non-existent phone {PhoneNumber}", phoneNumber);
                    return null;
                }

                // Only update if there are failed attempts or account is locked
                if (secret.FailedAttempts > 0 || secret.LockoutEnd.HasValue ) //|| secret.IsLocked)
                {
                    // Reset failed attempts
                    secret.FailedAttempts = 0;

                    // Unlock account if it was locked
                    // if (secret.IsLocked)
                    if (secret.LockoutEnd.HasValue && secret.LockoutEnd <= DateTime.UtcNow)
                    {
                        // secret.IsLocked = false;
                        secret.LockoutEnd = null;
                    }
                    else if (secret.LockoutEnd.HasValue && secret.LockoutEnd > DateTime.UtcNow)
                    {
                        // Account is still locked, do not unlock yet
                        _logger.LogWarning("Account for {PhoneNumber} is still locked until {LockoutEnd}", phoneNumber, secret.LockoutEnd);
                    }
                    else
                    {
                        // Account is not locked, just reset failed attempts
                        // secret.IsLocked = false;
                        secret.LockoutEnd = null;
                    }
                    // {
                    //     secret.IsLocked = false;
                    //     secret.LockoutEnd = null;

                    //     _logger.LogInformation("Account for {PhoneNumber} unlocked after successful verification", phoneNumber);
                    // }

                    // Record last successful use
                    secret.LastUsed = DateTime.UtcNow;

                    // Update in database
                    await _totpRepository.UpdateAsync(secret);
                }

                // Return DTO
                return new TOTPSecretDto
                {
                    PhoneNumber = secret.PhoneNumber,
                    SecretKey = null, // Don't return the actual secret key
                    CreatedAt = secret.CreatedAt,
                    IsActive = secret.IsActive,
                    IsLocked = false,
                    LockoutEnd = null,
                    FailedAttempts = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting failed attempts for {PhoneNumber}", phoneNumber);
                throw;
            }
        }

    }
}