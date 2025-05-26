using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SMSVotingSystem.Application.DTOs.AuthDtos;

namespace SMSVotingSystem.API.Controllers
{
    /*[ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        
        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _identityService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _identityService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _identityService.GetUsersAsync();
            return Ok(users);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<ActionResult> AssignRole(string userId, string role)
        {
            var result = await _identityService.AssignRoleAsync(userId, role);
            if (result)
            {
                return Ok();
            }
            
            return BadRequest("Failed to assign role");
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("remove-role")]
        public async Task<ActionResult> RemoveRole(string userId, string role)
        {
            var result = await _identityService.RemoveRoleAsync(userId, role);
            if (result)
            {
                return Ok();
            }
            
            return BadRequest("Failed to remove role");
        }
    }
    */
}