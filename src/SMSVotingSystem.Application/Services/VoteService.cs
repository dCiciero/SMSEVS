using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;

namespace SMSVotingSystem.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IVoterRepository _voterRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWork _unitOfWork;
        
        public VoteService(
            IVoteRepository voteRepository,
            IVoterRepository voterRepository,
            ICandidateRepository candidateRepository,
            IElectionRepository electionRepository,
            ISmsLogRepository smsLogRepository,
            ISmsService smsService,
            IUnitOfWork unitOfWork)
        {
            _voteRepository = voteRepository;
            _voterRepository = voterRepository;
            _candidateRepository = candidateRepository;
            _electionRepository = electionRepository;
            _smsLogRepository = smsLogRepository;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<SMSResponseDto> ProcessVoteBySmsAsync(string phoneNumber, string message)
        {
            // Log the incoming message
            // await LogSmsAsync(phoneNumber, message, SmsDirection.Inbound);
            
            // Find the voter
            var voter = await _voterRepository.GetByPhoneNumberAsync(phoneNumber);
            if (voter == null || !voter.IsRegistered)
            {
                // Voter not registered or inactive
                await _smsService.SendSmsAsync(phoneNumber, "You are not registered to vote. To register, send: REGISTER [IdNumber] [Your Name]");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "You are not registered to vote.To register, send: REGISTER [IdNumber] [Your Name]"
                };
            }
            
            // Find active election
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            if (activeElection == null || !activeElection.IsOngoing())
            {
                await _smsService.SendSmsAsync(phoneNumber, "There is no active election at this time.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "There is no active election at this time."
                };
            }
            
            // Check if already voted
            if (await _voterRepository.HasVotedInElectionAsync(voter.Id, activeElection.Id))
            {
                await _smsService.SendSmsAsync(phoneNumber, "You have already voted in this election.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "You have already voted in this election."
                };
            }

            // Parse the vote message to extract candidate code
            string candidateCode =message.Trim().ToUpperInvariant();  // message.Split(' ')[2]; //
            var candidate = await _candidateRepository.GetByShortCodeAsync(candidateCode);
            if (candidate == null)
            {
                await _smsService.SendSmsAsync(phoneNumber, $"Invalid candidate code: {candidateCode}. Please try again.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = $"Invalid candidate code: {candidateCode}. Please try again."
                };
            }
            
            // Record the vote
            var vote = new Vote(voter.Id, candidate.Id, activeElection.Id);
            await _voteRepository.AddAsync(vote);
            
            // Update voter's last voted timestamp
            voter.UpdateLastVoted();
            await _voterRepository.UpdateAsync(voter);
            
            await _unitOfWork.SaveChangesAsync();
            
            // Send confirmation
            await _smsService.SendSmsAsync(phoneNumber, $"Thank you for voting for {candidate.Name} in the {activeElection.Title} election.");
            
            return new SMSResponseDto
            {
                PhoneNumber = phoneNumber,
                Success = true,
                ResponseMessage = $"Thank you for voting for {candidate.Name} in the {activeElection.Title} election."
            };
        }
        
        public async Task<IEnumerable<VoteResultDto>> GetVoteResultsAsync(int electionId)
        {
            var votes = await _voteRepository.GetByElectionIdAsync(electionId);
            if (!votes.Any())
                return new List<VoteResultDto>();
                
            // Group votes by candidate
            var candidates = await _candidateRepository.GetAllAsync();
            var candidateDict = candidates.ToDictionary(c => c.Id);
            
            var voteGroups = votes.GroupBy(v => v.CandidateId);
            
            var results = new List<VoteResultDto>();
            int totalVotes = votes.Count();
            
            foreach (var group in voteGroups)
            {
                if (candidateDict.TryGetValue(group.Key, out var candidate))
                {
                    var voteCount = group.Count();
                    var percentage = totalVotes > 0 ? (double)voteCount / totalVotes * 100 : 0;
                    
                    results.Add(new VoteResultDto
                    {
                        CandidateId = candidate.Id,
                        CandidateName = candidate.Name,
                        CandidateShortCode = candidate.ShortCode,
                        VoteCount = voteCount,
                        Percentage = percentage
                    });
                }
            }
            
            return results.OrderByDescending(r => r.VoteCount);
        }
        
        
        private async Task LogSmsAsync(string phoneNumber, string message, SmsDirection direction)
        {
            var log = new SmsLog(phoneNumber, message, direction);
            await _smsLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<bool> HasVotedAsync(int voterId, int electionId)
        {
            throw new NotImplementedException();
        }
    }
}