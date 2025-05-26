using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.Common
{
    public class SecuritySettings
    {
        public string EncryptionKey { get; set; }
        public string EncryptionIV { get; set; }
        public int TOTPDigits { get; set; } = 6;
        public int TOTPTimeStepSeconds { get; set; } = 30;
        public int TOTPWindowSize { get; set; } = 1;
        public int MaxFailedAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 30;
        public int HOTPLookAheadWindow { get; set; } = 10;
    }
}