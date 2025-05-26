using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Domain.Services;

namespace SMSVotingSystem.Infrastructure.Services
{
    public class SmsServiceMock : ISmsService
    {
        public Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            // Mock sending SMS
            Console.WriteLine($"Mock SMS sent to {phoneNumber}: {message}");
            // return Task.CompletedTask;
            return Task.FromResult(true);
        }

        public Task<bool> SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message)
        {
            // Mock sending bulk SMS
            foreach (var number in phoneNumbers)
            {
                Console.WriteLine($"Mock SMS sent to {number}: {message}");
            }
            return Task.FromResult(true);
        }

        public Task<bool> CheckSmsStatusAsync(string messageId)
        {
            // Mock checking SMS status
            Console.WriteLine($"Mock status checked for message ID: {messageId}");
            return Task.FromResult(true);
        }
    }
}