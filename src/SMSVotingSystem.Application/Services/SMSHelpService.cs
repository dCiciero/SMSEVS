using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;
using static SMSVotingSystem.Application.DTOs.AuthDtos;

namespace SMSVotingSystem.Application.Services
{
    public class SMSHelpService : ISMSHelpService
    {
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly ISmsService _smsService;
        private readonly ITOTPService _totpService;
        private readonly IUnitOfWork _unitOfWork;

        public SMSHelpService(ISmsService smsService, ISmsLogRepository smsLogRepository, IUnitOfWork unitOfWork, ITOTPService totpService)
        {
            _smsService = smsService;
            _smsLogRepository = smsLogRepository;
            _unitOfWork = unitOfWork;
            _totpService = totpService;
        }

        public async Task<SMSResponseDto> ProcessHelpSmsAsync(SMSVerificationDto command)
        {
            // var verificationResult = await _totpService.VerifySmsCommandAsync(command);

            // if (!verificationResult.IsValid)
            // {
            //     await _smsService.SendSmsAsync(command.PhoneNumber, verificationResult.ResponseMessage);
            //     await LogSmsAsync(command.PhoneNumber, verificationResult.ResponseMessage, SmsDirection.Outbound);
            //     return new SMSResponseDto
            //     {
            //         PhoneNumber = command.PhoneNumber,
            //         Success = false,
            //         ResponseMessage = "Help information sent to your phone."
            //     };
            // }
            string helpMessage = "SMS Voting System Commands:\n" +
            "- Register: REG YOUR_ID YOUR_NAME \n" +
            "- Vote: VOTE YOUR_CODE CANDIDATE_ID\n" +
            "- Check Status: STATUS YOUR_CODE\n" +
            "- List Candidates: CANDIDATES YOUR_CODE\n" +
            "- Get Help: HOW\n" +
            "- Reset OTP: RESET YOUR_PHONE YOUR_ID";

            await _smsService.SendSmsAsync(command.PhoneNumber, helpMessage);
            await LogSmsAsync(command.PhoneNumber, helpMessage, SmsDirection.Outbound);

            return new SMSResponseDto
            {
                PhoneNumber = command.PhoneNumber,
                Success = true,
                ResponseMessage = "Help information sent to your phone."
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