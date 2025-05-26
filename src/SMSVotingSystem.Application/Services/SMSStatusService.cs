using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;
using static SMSVotingSystem.Application.DTOs.AuthDtos;

namespace SMSVotingSystem.Application.Services
{
    public class SMSStatusService : ISMSStatusService
    {
        private readonly IVoterRepository _voterRepository;
        private readonly ITOTPService _totpService;
        private readonly ISmsService _smsService;
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SMSStatusService> _logger;

        public SMSStatusService(IVoterRepository voterRepository, IEncryptionService encryptionService, IUnitOfWork unitOfWork, ILogger<SMSStatusService> logger, ITOTPService totpService, ISmsService smsService, ISmsLogRepository smsLogRepository)
        {
            _voterRepository = voterRepository;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _totpService = totpService;
            _smsService = smsService;
            _smsLogRepository = smsLogRepository;
        }
        public async Task<SMSResponseDto> ProcessStatusSmsAsync(SMSVerificationDto command)
        {
            // if (command.Command != "STATUS" || string.IsNullOrEmpty(command.OTP))
            // {
            //     return new SMSResponseDto
            //     {
            //         PhoneNumber = command.PhoneNumber,
            //         Success = false,
            //         ResponseMessage = "Invalid format. Text 'STATUS YOUR_CODE' to check your status."
            //     };
            // }

            var verificationResult = await _totpService.VerifySmsCommandAsync(command);

            if (!verificationResult.IsValid)
            {
                await _smsService.SendSmsAsync(command.PhoneNumber, verificationResult.ResponseMessage);
            }

            // Verify TOTP
            // bool isValidOTP = await _totpService.VerifyTOTPAsync(command.PhoneNumber, command.OTP);
            // if (!isValidOTP)
            // {
            //     await smsService.SendSmsAsync(command.PhoneNumber, "Invalid verification code. Please try again with the correct 6-digit code.");
            //     return new SMSResponseDto
            //     {
            //         PhoneNumber = command.PhoneNumber,
            //         Success = false,
            //         ResponseMessage = "Invalid verification code. Please try again with the correct 6-digit code."
            //     };
            // }

            // Get voter status
            var voter = await _voterRepository.GetByPhoneNumberAsync(command.PhoneNumber);
            if (voter == null)
            {
                await _smsService.SendSmsAsync(command.PhoneNumber, "You are not registered in the system.");
                await LogSmsAsync(command.PhoneNumber, "You are not registered in the system.", SmsDirection.Outbound);
                return new SMSResponseDto
                {
                    PhoneNumber = command.PhoneNumber,
                    Success = false,
                    ResponseMessage = "You are not registered in the system."
                };
            }

            string status = voter.LastVoted != null
                ? "You have already voted in this election."
                : "You are registered but have not yet voted.";

            await _smsService.SendSmsAsync(command.PhoneNumber, status);
            await LogSmsAsync(command.PhoneNumber, status, SmsDirection.Outbound);
            return new SMSResponseDto
            {
                PhoneNumber = command.PhoneNumber,
                Success = true,
                ResponseMessage = $"Voter Status: {status}"
            };
        }

        private async Task LogSmsAsync(string phoneNumber, string message, SmsDirection direction)
        {
            var log = new SmsLog(phoneNumber, message, direction);
            await _smsLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}