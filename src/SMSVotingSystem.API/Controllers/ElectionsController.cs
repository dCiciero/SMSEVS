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
    public class ElectionsController : ControllerBase
    {
        private readonly IElectionService _electionService;
        private readonly INotificationService _notificationService;

        public ElectionsController(
            IElectionService electionService,
            INotificationService notificationService)
        {
            _electionService = electionService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ElectionDto>>> GetElections()
        {
            var elections = await _electionService.GetAllElectionsAsync();
            return Ok(elections);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ElectionDto>> GetElection(int id)
        {
            var election = await _electionService.GetElectionByIdAsync(id);
            if (election == null)
                return NotFound();

            return Ok(election);
        }

        [HttpGet("active")]
        public async Task<ActionResult<ElectionDto>> GetActiveElection()
        {
            var election = await _electionService.GetActiveElectionAsync();
            if (election == null)
                return NotFound();

            return Ok(election);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ElectionDto>> CreateElection(CreateElectionDto createElectionDto)
        {
            try
            {
                var election = await _electionService.CreateElectionAsync(createElectionDto);
                return CreatedAtAction(nameof(GetElection), new { id = election.Id }, election);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ElectionDto>> UpdateElection(int id, UpdateElectionDto updateElectionDto)
        {
            try
            {
                var election = await _electionService.UpdateElectionAsync(id, updateElectionDto);
                return Ok(election);
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

        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}/activate")]
        public async Task<ActionResult<ElectionDto>> ActivateElection(int id)
        {
            try
            {
                var election = await _electionService.ActivateElectionAsync(id);

                // Notify all registered voters about the new active election
                await _notificationService.NotifyVotersAboutElectionAsync(id);

                return Ok(election);
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

        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult<ElectionDto>> DeactivateElection(int id)
        {
            try
            {
                var election = await _electionService.DeactivateElectionAsync(id);
                return Ok(election);
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

        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteElection(int id)
        {
            try
            {
                await _electionService.DeleteElectionAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is Application.Common.Exceptions.NotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}