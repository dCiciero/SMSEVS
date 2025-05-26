using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface ISmsLogService
    {
        /// <summary>
        /// Get all SMS logs
        /// </summary>
        /// <returns>A collection of SMS log DTOs</returns>
        Task<IEnumerable<SmsLogDto>> GetAllLogsAsync();

        /// <summary>
        /// Get the most recent SMS logs
        /// </summary>
        /// <param name="count">The number of logs to retrieve</param>
        /// <returns>A collection of the most recent SMS log DTOs</returns>
        Task<IEnumerable<SmsLogDto>> GetRecentLogsAsync(int count);

        Task<SMSResponseDto> ProcessIncomingSmsAsync(string phoneNumber, string messageContent);
        // Task<SmsLogDto> ProcessIncomingSmsAsync(string phoneNumber, string messageContent);
    }
}