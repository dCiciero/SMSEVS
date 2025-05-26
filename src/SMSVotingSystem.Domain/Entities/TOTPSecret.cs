using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Entities
{
    public class TOTPSecret
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string EncryptedSecretKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsed { get; set; }
        public int Counter { get; set; }
        public bool IsActive { get; set; }
        public int FailedAttempts { get; set; }
        //public bool IsLocked { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}