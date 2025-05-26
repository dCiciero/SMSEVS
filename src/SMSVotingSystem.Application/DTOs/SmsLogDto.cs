using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.DTOs
{
    public class SmsLogDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
        public string Direction { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProcessingStatus { get; set; }
    }
}