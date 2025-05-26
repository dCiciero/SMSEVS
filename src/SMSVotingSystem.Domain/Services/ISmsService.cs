using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Services
{
    public interface ISmsService
    {
        /// <summary>
        /// Send an SMS message to a phone number
        /// </summary>
        /// <param name="toPhoneNumber">The recipient's phone number</param>
        /// <param name="message">The message content</param>
        /// <returns>True if message was sent successfully, false otherwise</returns>
        Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    }
}