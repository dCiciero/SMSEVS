using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.Common;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Domain.Services;

namespace SMSVotingSystem.Application.Services
{
    public class SmsLogService : ISmsLogService
    {
        private readonly ISmsLogRepository _smsLogRepository;
        private readonly ISmsService _smsService;
        private readonly IVoteService _voteService;
        private readonly ISMSStatusService _smsStatusService;
        private readonly ISMSHelpService _smsHelpService;
        private readonly IElectionService _electionService;
        private readonly IRegistrationService _registrationService;
        private readonly ITOTPService _totpService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVoterService _voterService;

        public SmsLogService(ISmsLogRepository smsLogRepository, IUnitOfWork unitOfWork, ISmsService smsService, ITOTPService totpService, IRegistrationService registrationService, ISMSHelpService smsHelpService, ISMSStatusService smsStatusService, IVoteService voteService, IElectionService electionService, IVoterService voterService)
        {
            _smsLogRepository = smsLogRepository;
            _unitOfWork = unitOfWork;
            _smsService = smsService;
            _totpService = totpService;
            _registrationService = registrationService;
            _smsHelpService = smsHelpService;
            _smsStatusService = smsStatusService;
            _voteService = voteService;
            _electionService = electionService;
            _voterService = voterService;
        }

        public async Task<IEnumerable<SmsLogDto>> GetAllLogsAsync()
        {
            var logs = await _smsLogRepository.GetAllAsync();
            return logs.Select(l => new SmsLogDto
            {
                Id = l.Id,
                PhoneNumber = l.PhoneNumber,
                MessageText = l.MessageText,
                Direction = l.Direction.ToString(),
                Timestamp = l.Timestamp,
                ProcessingStatus = l.ProcessingStatus
            });
        }

        public async Task<IEnumerable<SmsLogDto>> GetRecentLogsAsync(int count)
        {
            var logs = await _smsLogRepository.GetRecentLogsAsync(count);
            return logs.Select(l => new SmsLogDto
            {
                Id = l.Id,
                PhoneNumber = l.PhoneNumber,
                MessageText = l.MessageText,
                Direction = l.Direction.ToString(),
                Timestamp = l.Timestamp,
                ProcessingStatus = l.ProcessingStatus
            });
        }

        // public async Task<SmsLogDto> ProcessIncomingSmsAsync(string phoneNumber, string messageContent)
        public async Task<SMSResponseDto> ProcessIncomingSmsAsync(string phoneNumber, string messageContent)
        {
            try
            {
                try
                {
                    await LogSmsAsync(phoneNumber, messageContent, SmsDirection.Inbound);
                }
                catch (Exception logEx)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine($"Failed to log SMS: {logEx.Message}");
                }

                SMSVerificationResultDto verificationResult;
                var verification = new SMSVerificationDto
                {
                    PhoneNumber = phoneNumber,
                    Message = messageContent
                };


                 verificationResult = await _totpService.VerifySmsCommandAsync(verification);
                 
                // call _totpService.VerifySmsCommandAsync(verification) to verify the command if command is either "REG" or "REGISTER" and "Vote"
                // if (verification.Command.ToUpper() == "REG" || verification.Command.ToUpper() == "REGISTER" || verification.Command.ToUpper() == "VOTE")
                // {
                //     try
                //     {
                //         verificationResult = await _totpService.VerifySmsCommandAsync(verification);
                //     }
                //     catch (Exception ex)
                //     {
                //         await _smsService.SendSmsAsync(phoneNumber, "An error occurred while verifying your command. Please try again.");
                //         await LogSmsAsync(phoneNumber, "An error occurred while verifying the command.", SmsDirection.Outbound);
                //         return new SMSResponseDto
                //         {
                //             PhoneNumber = phoneNumber,
                //             Success = false,
                //             ResponseMessage = "An error occurred while verifying your command. Please try again."
                //         };
                //     }
                // }

                // else
                // {
                //     verificationResult = new SMSVerificationResultDto
                //     {
                //         IsValid = true
                //     };
                // }

                if (!verificationResult.IsValid)
                {
                    await _smsService.SendSmsAsync(phoneNumber, verificationResult.ResponseMessage);
                    await LogSmsAsync(phoneNumber, verificationResult.ResponseMessage, SmsDirection.Outbound);
                    return new SMSResponseDto
                    {
                        PhoneNumber = phoneNumber,
                        Success = false,
                        ResponseMessage = verificationResult.ResponseMessage
                    };

                }



                // Process command
                switch (verification.Command.ToUpper())
                {
                    case "REG":
                    case "REGISTER":
                        if (verification.OTP == null && verification.Parameters != null && verification.Parameters.Length >= 2)
                        {
                            // New registration
                            var registrationResult = await _registrationService.ProcessRegistrationRequestAsync(phoneNumber, messageContent);
                            return registrationResult;

                        }
                        else
                        {
                            return await SendErrorResponseAsync(phoneNumber, "Invalid registration format. Please send: REGISTER [IdNumber] [Your Name]");
                        }
                    // break;
                    case "VOTE":
                        var voteResult = await _voteService.ProcessVoteBySmsAsync(phoneNumber, verification.Parameters[0]);
                        if (voteResult.Success)
                        {
                            await _smsService.SendSmsAsync(phoneNumber, voteResult.ResponseMessage);
                            await LogSmsAsync(phoneNumber, voteResult.ResponseMessage, SmsDirection.Outbound);
                        }
                        else
                        {
                            await SendErrorResponseAsync(phoneNumber, voteResult.ResponseMessage);
                        }
                        return voteResult;
                    // break;
                    // case "STATUS":
                    //     await _smsStatusService.ProcessStatusSmsAsync(verification);
                    //     break;
                    // case "RESULT":
                    //     await _voteService.ProcessVoteBySmsAsync(phoneNumber, messageContent);
                    //     break;
                    case "HOW":
                        var helpResult = await _smsHelpService.ProcessHelpSmsAsync(verification);
                        return helpResult;

                    case "RESET":
                        var resetResult = await _smsHelpService.ProcessHelpSmsAsync(verification);
                        return resetResult;
                    case "CANDIDATES":
                    case "CAND":
                        var candidateList = await ProcessCandidateListAsync(phoneNumber);
                        return candidateList;
                    case "CODE":
                        var codeResult = await ProcessCodeRequestAsync(phoneNumber);
                        return codeResult;

                    default:
                        // Handle unknown command
                        return await SendErrorResponseAsync(phoneNumber, "Unknown command. Please send 'HELP' for assistance.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                throw;
            }

        }

        private async Task<SMSResponseDto> ProcessCandidateListAsync(string phoneNumber)
        {
           //throw new NotImplementedException("Candidate list processing is not implemented yet.");
            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
            {
                await _smsService.SendSmsAsync(phoneNumber, "There is no active election at the moment.");
                // throw new Exception($"Error processing incoming SMS from {phoneNumber} with message '{messageContent}': {ex.Message}", ex);
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "There is no active election at the moment."
                };
            }

            var candidates = await _electionService.GetCandidatesForElectionAsync(activeElection.Id);
            if (candidates == null || !candidates.Any())
            {
                // No candidates registered for the current election
                await _smsService.SendSmsAsync(phoneNumber, "No candidates are registered for the current election.");
                // Log the response
                await LogSmsAsync(phoneNumber, "No candidates are registered for the current election.", SmsDirection.Outbound);
                // return await CreateResponseAsync(phoneNumber, "Success", "No candidates are registered for the current election.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "No candidates are registered for the current election."
                };
                //throw new Exception($"Error processing incoming SMS from {phoneNumber} with message '{messageContent}': {ex.Message}", ex);
            }
            // {
            //     await _smsService.SendSmsAsync(phoneNumber, "No candidates are registered for the current election.");
            //     return await CreateResponseAsync(phoneNumber, "Success", "No candidates are registered for the current election.");
            // }

            // Format candidate list for SMS
            string candidateList = "Candidates:\n";
            foreach (var candidate in candidates)
            {
                candidateList += $"{candidate.Id}: {candidate.Name} - Code: {candidate.ShortCode}\n";
            }

            candidateList += "\nTo vote, text: VOTE YOUR_CODE CANDIDATE_CODE";
            await _smsService.SendSmsAsync(phoneNumber, candidateList);
            await LogSmsAsync(phoneNumber, candidateList, SmsDirection.Outbound);
            // return await CreateResponseAsync(phoneNumber, "Success", candidateList);
            return new SMSResponseDto
            {
                PhoneNumber = phoneNumber,
                Success = true,
                ResponseMessage = candidateList  
            };
            //return new SMSResponseDto

            // return await CreateResponseAsync(phoneNumber, "Success", candidateList);
        }

        private async Task<SMSResponseDto> ProcessCodeRequestAsync(string phoneNumber)
        {
            try
            {
                var voter = await _voterService.GetVoterByPhoneNumberAsync(phoneNumber);
                if (voter == null)
                {
                    return new SMSResponseDto
                    {
                        PhoneNumber = phoneNumber,
                        Success = false,
                        ResponseMessage = "You are not registered in the system."
                    };
                }
                //     return await CreateResponseAsync(phoneNumber, "Error", "You are not registered in the system.");
                // }

                // Get the voter's secret
                var totpSecret = await _totpService.GetSecretForPhoneAsync(phoneNumber);
                if (totpSecret == null)
                {
                    return new SMSResponseDto
                    {
                        PhoneNumber = phoneNumber,
                        Success = false,
                        ResponseMessage = "Your security settings were not found. Please contact support."
                    };
                    //return await CreateResponseAsync(phoneNumber, "Error", "Your security settings were not found. Please contact support.");
                }

                // Generate the current verification code
                string currentCode = TOTPUtility.GenerateCurrentTOTP(totpSecret.SecretKey);

                // Calculate how many seconds this code remains valid
                int secondsRemaining = TOTPUtility.GetRemainingSecondsForCurrentTimeStep();

                await LogSmsAsync(phoneNumber,
                    $"Your verification code is: {currentCode}\n" +
                    $"This code is valid for the next {secondsRemaining} seconds.\n" +
                    $"Use this code for your voting commands.", SmsDirection.Outbound);

                // Send the verification code to the user
                await _smsService.SendSmsAsync(phoneNumber,
                    $"Your verification code is: {currentCode}\n" +
                    $"This code is valid for the next {secondsRemaining} seconds.\n" +
                    $"Use this code for your voting commands.");

                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = true,
                    ResponseMessage = $"Your verification code is: {currentCode}\n" +
                                      $"This code is valid for the next {secondsRemaining} seconds.\n" +
                                      $"Use this code for your voting commands."
                };
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error generating code for {PhoneNumber}", phoneNumber);
                await SendErrorResponseAsync(phoneNumber, "An error occurred while generating your code. Please try again.");
                return new SMSResponseDto
                {
                    PhoneNumber = phoneNumber,
                    Success = false,
                    ResponseMessage = "An error occurred while generating your code. Please try again."
                };

            }
        }
        private async Task LogSmsAsync(string phoneNumber, string message, SmsDirection direction)
        {
            var log = new SmsLog(phoneNumber, message, direction);
            await _smsLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<SMSResponseDto> SendErrorResponseAsync(string phoneNumber, string responseMessage)
        {
            await _smsService.SendSmsAsync(phoneNumber, responseMessage);
            await LogSmsAsync(phoneNumber, responseMessage, SmsDirection.Outbound);
            return new SMSResponseDto
            {
                PhoneNumber = phoneNumber,
                Success = false,
                ResponseMessage = responseMessage
            };
        }
    }
}