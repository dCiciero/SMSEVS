using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.DTOs
{
    public class AuthDtos
    {
        
    }

    public class UserDto
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<string> Roles { get; set; }
        }

        public class LoginDto
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class RegisterDto
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AuthResponseDto
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
            public DateTime Expiration { get; set; }
            public UserDto User { get; set; }
        }

        public class TOTPSecretDto
        {
            public string PhoneNumber { get; set; }
            public string SecretKey { get; set; }
            public DateTime CreatedAt { get; set; }
            public int Counter { get; set; }
            public bool IsActive { get; set; }
            public bool? IsLocked { get; set; }
            public int FailedAttempts { get; set; }
            public DateTime? LockoutEnd { get; set; }

        }

        public class SMSVerificationDto
        {
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
            public string OTP { get; set; }
            public string Command { get; set; }
            public string[] Parameters { get; set; }
        }

        public class SMSVerificationResultDto
        {
            public bool IsValid { get; set; }
            public string ResponseMessage { get; set; }
        }

        public class SMSResponseDto
        {
            public string PhoneNumber { get; set; }
            public string ResponseMessage { get; set; }
            public bool Success { get; set; }
            public string ErrorCode { get; set; }
        }

        public class SMSCommandDto
        {
            public string PhoneNumber { get; set; }
            public string RawMessage { get; set; }
            public string Command { get; set; }
            public string OTP { get; set; }
            public string[] Parameters { get; set; }
        }
}