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
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;
        
        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateDto>>> GetCandidates()
        {
            var candidates = await _candidateService.GetAllCandidatesAsync();
            return Ok(candidates);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateDto>> GetCandidate(int id)
        {
            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            if (candidate == null)
                return NotFound();
                
            return Ok(candidate);
        }
        
        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CandidateDto>> CreateCandidate(CreateCandidateDto createCandidateDto)
        {
            try
            {
                var candidate = await _candidateService.CreateCandidateAsync(createCandidateDto);
                return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CandidateDto>> UpdateCandidate(int id, UpdateCandidateDto updateCandidateDto)
        {
            try
            {
                var candidate = await _candidateService.UpdateCandidateAsync(id, updateCandidateDto);
                return Ok(candidate);
            }
            catch (Exception ex) when (ex is Application.Common.Exceptions.NotFoundException)
            {
                return NotFound();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCandidate(int id)
        {
            try
            {
                await _candidateService.DeleteCandidateAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex is Application.Common.Exceptions.NotFoundException)
            {
                return NotFound();
            }
        }
    }
}