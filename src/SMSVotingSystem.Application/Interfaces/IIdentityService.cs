using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.DTOs;
using static SMSVotingSystem.Application.DTOs.AuthDtos;

namespace SMSVotingSystem.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> RemoveRoleAsync(string userId, string role);
    }
}