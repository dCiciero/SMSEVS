using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;

namespace SMSVotingSystem.Infrastructure.Repositories
{
    public class TOTPRepository : ITOTPRepository
    {
        private readonly ApplicationDbContext _context;

        public TOTPRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TOTPSecret> CreateAsync(TOTPSecret secret)
        {
            _context.TOTPSecrets.Add(secret);
            await _context.SaveChangesAsync();
            return secret;
        }

        public async Task<TOTPSecret> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.TOTPSecrets
                .FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber);
        }

        public async Task<bool> RecordFailedAttemptAsync(string phoneNumber)
        {
            var secret = await GetByPhoneNumberAsync(phoneNumber);
            if (secret == null) return false;

            secret.FailedAttempts++;
            
            // If max attempts reached, lock account
            if (secret.FailedAttempts >= 5) // This could be configurable
            {
                secret.LockoutEnd = DateTime.UtcNow.AddMinutes(30); // 30 min lockout
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetFailedAttemptsAsync(string phoneNumber)
        {
            var secret = await GetByPhoneNumberAsync(phoneNumber);
            if (secret == null) return false;

            secret.FailedAttempts = 0;
            secret.LockoutEnd = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateAsync(TOTPSecret secret)
        {
            _context.TOTPSecrets.Update(secret);
            await _context.SaveChangesAsync();
        }
    }
}