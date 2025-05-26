using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;

namespace SMSVotingSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ISmsService _smsService;

        public NotificationService(
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository,
            IElectionRepository electionRepository,
            ISmsService smsService)
        {
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _electionRepository = electionRepository;
            _smsService = smsService;
        }

        public async Task NotifyVotersAboutElectionAsync(int electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null || !election.IsActive)
            {
                return;
            }

            var candidates = await _candidateRepository.GetAllAsync();

            // Build the message with candidate information
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"A new election is now active: {election.Title}");
            messageBuilder.AppendLine("To vote, send a text with the candidate's code:");

            foreach (var candidate in candidates)
            {
                messageBuilder.AppendLine($"{candidate.ShortCode} - {candidate.Name}");
            }

            string message = messageBuilder.ToString();

            // Get all registered voters
            var voters = await _voterRepository.GetAllAsync();
            var registeredVoters = voters.Where(v => v.IsRegistered);

            // Send notifications (could be improved with batch processing for large voter bases)
            foreach (var voter in registeredVoters)
            {
                await _smsService.SendSmsAsync(voter.PhoneNumber, message);
            }
        }

        public async Task SendRegistrationConfirmationAsync(string phoneNumber, string fullName, string secretKey, string currentCode)
        {
            string message = $"Hello {fullName}, your registration is successful.\n\n" +
                // $"Your personal security code is: {secretKey}\n" +
                // $"Keep this SMS safe or write down your code somewhere secure.\n\n" +
                $"Current verification code: {currentCode}\n" +
                $"This verification code changes every 60 seconds.\n\n" +
                $"How to use:\n" +
                $"1. For all SMS commands, you'll need to include a verification code\n" +
                // $"2. Get your verification code from a TOTP app like Google Authenticator\n" +
                // $"3. Set up the app by entering your security code\n" +
                $"2. Text 'CODE' to this number to get your current verification code\n\n" +
                $"Example: To vote, send 'VOTE 123456 CANDIDATE_ID' where 123456 is your current verification code\n\n" +
                $"Need help? Text 'HOW' to this number.";

                string message2 = $"Hello {fullName}, your registration is successful.\n\n" +
                $"Your personal security code is: {secretKey}\n" +
                $"Keep this SMS safe or write down your code somewhere secure.\n\n" +
                $"Current verification code: {currentCode}\n" +
                $"This verification code changes every 30 seconds.\n\n" +
                $"How to use:\n" +
                $"1. For all SMS commands, you'll need to include a verification code\n" +
                $"2. Get your verification code from a TOTP app like Google Authenticator\n" +
                $"3. Set up the app by entering your security code\n" +
                $"4. Or, text 'CODE' to this number to get your current verification code\n\n" +
                $"Example: To vote, send 'VOTE 123456 CANDIDATE_ID' where 123456 is your current verification code\n\n" +
                $"Need help? Text 'HELP' to this number.";

            await _smsService.SendSmsAsync(phoneNumber, message);
        }

        public Task SendVoteConfirmationAsync(string phoneNumber, string fullName, string electionTitle, string candidateName, string candidateCode)
        {
            throw new NotImplementedException();
        }
    }
}