using System;
using System.Threading.Tasks;
using SMSVotingSystem.Application.Common;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;

namespace SMSVotingSystem.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IVoterRepository _voterRepository;
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly ISmsService _smsService;
        private readonly ITOTPService _totpService;
        
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationService(
            IVoterRepository voterRepository,
            ISmsLogRepository smsLogRepository,
            ISmsService smsService,
            IUnitOfWork unitOfWork,
            ITOTPService totpService,
            INotificationService notificationService)
        {
            _voterRepository = voterRepository;
            _smsLogRepository = smsLogRepository;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
            _totpService = totpService;
            _notificationService = notificationService;
        }

        public async Task<SMSResponseDto> ProcessRegistrationRequestAsync(string phoneNumber, string message)
        {
            // Log the incoming message
            //await LogSmsAsync(phoneNumber, message, SmsDirection.Inbound);

            // Check if voter is already registered
            var existingVoter = await _voterRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existingVoter != null)
            {
                await _smsService.SendSmsAsync(phoneNumber, "You are already registered in our system.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "You are already registered in our system."
                };
            }

            

            // Parse registration message
            // Expected format: "REGISTER [IDNumber] [Name]"
            // string[] parts = message.Trim().Split(new[] { ' ' }, 3);
            var parts = message.Trim().Split(' ' , StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3 || (!parts[0].Equals("REGISTER", StringComparison.OrdinalIgnoreCase) && !parts[0].Equals("REG", StringComparison.OrdinalIgnoreCase)))
            {
                await _smsService.SendSmsAsync(phoneNumber, "Invalid registration format. Please send: REGISTER [Your Name]");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "Invalid registration format. Please send: REGISTER [IdNumber] [Your Name]"
                };
            }

            string idNumber = parts[1].Trim();
            if (string.IsNullOrWhiteSpace(idNumber))
            {
                await _smsService.SendSmsAsync(phoneNumber,
                    "ID number cannot be empty. Please send: REGISTER [IdNumber] [Your Name]");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "ID number cannot be empty. Please send: REGISTER [IdNumber] [Your Name]"
                };
            }
            if (!IsValidIdNumber(idNumber))
            {
                await _smsService.SendSmsAsync(phoneNumber,
                    "Invalid ID format. It must be alphanumeric and between 5 and 15 characters long. " +
                    "Please send: REGISTER [IdNumber] [Your Name]");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    // ResponseMessage = "Invalid ID format. It must contain numbers and be alphanumeric. and between 5 and 15 characters long. " +
                    ResponseMessage = "Invalid ID format. It must be alphanumeric and between 5 and 15 characters long. " +
                                      "Please send: REGISTER [IdNumber] [Your Name]"
                };
            }

            var existingIdNumber = await _voterRepository.IdNumberExistsAsync(idNumber);
            if (existingIdNumber)
            {
                await _smsService.SendSmsAsync(phoneNumber, "This ID number is already registered.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "This ID number is already registered."
                };
            }

            var nameParts = parts.Skip(2);
            var voterName = string.Join(' ', nameParts);
            // string voterName = parts[2].Trim();
            if (string.IsNullOrWhiteSpace(voterName))
            {
                await _smsService.SendSmsAsync(phoneNumber,
                    "Name cannot be empty. Please send: REGISTER [IdNumber] [Your Name]");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "Name cannot be empty. Please send: REGISTER [IdNumber] [Your Name]"
                };
            }

            // Create new voter
            var newVoter = new Voter(phoneNumber, idNumber, voterName);
            await _voterRepository.AddAsync(newVoter);
            await _unitOfWork.SaveChangesAsync();

            // Generate TOTP secret
            var totpSecret = await _totpService.GenerateSecretAsync(phoneNumber);


 
            string currentCode = TOTPUtility.GenerateCurrentTOTP(totpSecret.SecretKey);

            //Send confirmation with the secret
            await _notificationService.SendRegistrationConfirmationAsync(phoneNumber, voterName, totpSecret.SecretKey, currentCode);
            // await _smsService.SendSmsAsync(phoneNumber,
            //     $"Thank you {voterName}! You have been successfully registered to vote. " +
            //     $"Your security code is: {secret.SecretKey}. \n\n" +
            //     "Keep this SMS safe. For all future commands, you'll need the 6-digit verification code. " +
            //     "Example: VOTE 123456 CANDIDATE_ID" +
            //     "\n\n" +
            //     "When an election is active, you will receive instructions on how to cast your vote.");

            return new SMSResponseDto
            {
                PhoneNumber = phoneNumber,
                Success = true,
                ResponseMessage = "Registration successful! A confirmation SMS has been sent."
            };
        }

        private async Task LogSmsAsync(string phoneNumber, string message, SmsDirection direction)
        {
            var log = new SmsLog(phoneNumber, message, direction);
            await _smsLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        private bool IsValidIdNumber(string idNumber)
        {
            bool containsDigit = idNumber.Any(char.IsDigit);
            bool isOnlyLetters = idNumber.All(char.IsLetter);
            bool isAlphanumeric = idNumber.All(char.IsLetterOrDigit);
            bool isValidLength = idNumber.Length >= 5 && idNumber.Length <= 15;

            return containsDigit && isAlphanumeric && !isOnlyLetters && isValidLength;
        }
    }
}