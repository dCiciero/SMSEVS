using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SMSVotingSystem.Infrastructure.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;
        
        public TwilioSmsService(
            ISmsLogRepository smsLogRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _smsLogRepository = smsLogRepository;
            _unitOfWork = unitOfWork;
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromPhoneNumber = configuration["Twilio:PhoneNumber"];
        }
        
        public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                // Set up Twilio client
                TwilioClient.Init(_accountSid, _authToken);
                
                // Send the message
                var twilioMessage = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );
                
                // Log the outbound message
                var log = new SmsLog(toPhoneNumber, message, SmsDirection.Outbound, "Sent");
                await _smsLogRepository.AddAsync(log);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                var log = new SmsLog(toPhoneNumber, message, SmsDirection.Outbound, $"Failed: {ex.Message}");
                await _smsLogRepository.AddAsync(log);
                await _unitOfWork.SaveChangesAsync();
                
                return false;
            }
        }
    }
}