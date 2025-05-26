using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SMSVotingSystem.Application.DTOs.AuthDtos;

namespace SMSVotingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly IVoteService _voteService;
        private readonly IRegistrationService _registrationService;
        private readonly ISmsLogService _smsLogService;
        // private readonly ISMSStatusService _smsStatusService;
        // private readonly ISMSHelpService _smsHelpService;
        // // private readonly ITOTPService _totpService;
        private readonly ILogger<SmsController> _logger;

        public SmsController(
            IVoteService voteService,
            IRegistrationService registrationService,
            ISmsLogService smsLogService,
            ILogger<SmsController> logger
            // // ITOTPService totpService,
            // ISMSStatusService smsStatusService,
            // ISMSHelpService smsHelpService
            )
        {
            _voteService = voteService;
            _registrationService = registrationService;
            _smsLogService = smsLogService;
            _logger = logger;
            // // _totpService = totpService;
            // _smsStatusService = smsStatusService;
            // _smsHelpService = smsHelpService;
        }

        // Endpoint for Twilio webhook
        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveSms([FromForm] string From, [FromForm] string Body)
        {
            _logger.LogInformation($"Received SMS: From={From}, Body={Body}");

            // Process the incoming SMS
            string phoneNumber = From;
            string message = Body;
            await _smsLogService.ProcessIncomingSmsAsync(phoneNumber, message);

            // // Check if this is a registration message
            // if (message.Trim().StartsWith("REGISTER", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _registrationService.ProcessRegistrationRequestAsync(phoneNumber, message);
            // }


            // var verification = new SMSVerificationDto
            // {
            //     PhoneNumber = phoneNumber,
            //     Message = message
            // };
            // if (message.Trim().StartsWith("VOTE", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _voteService.ProcessVoteBySmsAsync(phoneNumber, message);
            // }

            // else if (message.Trim().StartsWith("STATUS", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _smsStatusService.ProcessStatusSmsAsync(verification);
            //     // await _smsStatusService.ProcessStatusSmsAsync(phoneNumber, message);
            // }
            // else if (message.Trim().StartsWith("RESULT", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _voteService.ProcessVoteBySmsAsync(phoneNumber, message);
            // }
            // // else if (message.Trim().StartsWith("UNREGISTER", StringComparison.OrdinalIgnoreCase))
            // // {
            // //     await _voteService.ProcessVoteBySmsAsync(phoneNumber, message);
            // // }
            // else if (message.Trim().StartsWith("HELP", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _smsHelpService.ProcessHelpSmsAsync(verification);
            // }

            /********************************************************************/
            // // Prepare verification DTO
            // var verification = new SMSVerificationDto
            // {
            //     PhoneNumber = phoneNumber,
            //     Message = message
            // };

            // // Verify the SMS command with TOTP
            // var verificationResult = await _totpService.VerifySmsCommandAsync(verification);

            // if (!verificationResult.IsValid)
            // {
            //     return await CreateResponseAsync(phoneNumber, "Error", verificationResult.ErrorMessage);
            // }

            // Return empty TwiML response
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response></Response>", "application/xml");
        }

        [HttpGet("logs")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SmsLogDto>>> GetSmsLogs()
        {
            var logs = await _smsLogService.GetAllLogsAsync();
            return Ok(logs);
        }

        [HttpGet("logs/recent/{count}")]
        // [Authorize]
        public async Task<ActionResult<IEnumerable<SmsLogDto>>> GetRecentSmsLogs(int count)
        {
            var logs = await _smsLogService.GetRecentLogsAsync(count);
            return Ok(logs);
        }

        // Simulation endpoint for testing
        [HttpPost("simulate")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SimulateSms([FromBody] SimulatedSmsModel model)
        {
            _logger.LogInformation($"Simulating SMS: From={model.From}, Body={model.Body}");

            // Process the simulated SMS
            string phoneNumber = model.From;
            string message = model.Body;
            var res = await _smsLogService.ProcessIncomingSmsAsync(phoneNumber, message);
            //check res details and return appropriate response
            if (res.Success)
            {
                return Ok(res.ResponseMessage);
            }
            else
            {
                return BadRequest(res.ResponseMessage);
            }
            // Check if this is a registration message



            // if (model.Body.StartsWith("REGISTER", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _registrationService.ProcessRegistrationRequestAsync(model.From, model.Body);
            // }
            // else
            // {
            //     await _voteService.ProcessVoteBySmsAsync(model.From, model.Body);
            // }


            //return Ok();
        }
    }

    public class SimulatedSmsModel
    {
        public required string From { get; set; }
        public required string Body { get; set; }
    }
}