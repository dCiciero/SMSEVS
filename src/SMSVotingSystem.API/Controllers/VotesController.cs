using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMSVotingSystem.API.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;
        
        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }
        
        [HttpGet("results/{electionId}")]
        public async Task<ActionResult<IEnumerable<VoteResultDto>>> GetVoteResults(int electionId)
        {
            var results = await _voteService.GetVoteResultsAsync(electionId);
            return Ok(results);
        }
    }
}