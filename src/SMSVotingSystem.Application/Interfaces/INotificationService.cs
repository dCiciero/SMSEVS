using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Notify all registered voters about an active election
        /// </summary>
        /// <param name="electionId">The ID of the election to notify about</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task NotifyVotersAboutElectionAsync(int electionId);

        Task SendRegistrationConfirmationAsync(string phoneNumber, string fullName, string secretKey, string currentCode);
        Task SendVoteConfirmationAsync(string phoneNumber, string fullName, string electionTitle, string candidateName, string candidateCode);
    }
}