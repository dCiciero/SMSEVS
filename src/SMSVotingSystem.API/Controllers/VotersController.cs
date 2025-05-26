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
    public class VotersController : ControllerBase
    {
        private readonly IVoterService _voterService;

        public VotersController(IVoterService voterService)
        {
            _voterService = voterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VoterDto>>> GetVoters()
        {
            var voters = await _voterService.GetAllVotersAsync();
            return Ok(voters);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VoterDto>> GetVoter(int id)
        {
            var voter = await _voterService.GetVoterByIdAsync(id);
            if (voter == null)
                return NotFound();

            return Ok(voter);
        }

        [HttpPost]
        public async Task<ActionResult<VoterDto>> RegisterVoter(CreateVoterDto createVoterDto)
        {
            try
            {
                var voter = await _voterService.RegisterVoterAsync(createVoterDto);
                return CreatedAtAction(nameof(GetVoter), new { id = voter.Id }, voter);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VoterDto>> UpdateVoter(int id, UpdateVoterDto updateVoterDto)
        {
            try
            {
                var voter = await _voterService.UpdateVoterAsync(id, updateVoterDto);
                return Ok(voter);
            }
            catch (Exception ex) when (ex is Application.Common.Exceptions.NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVoter(int id)
        {
            try
            {
                await _voterService.DeleteVoterAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is Application.Common.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }
    }
}