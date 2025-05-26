using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.DTOs
{
    public class VoteResultDto
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateShortCode { get; set; }
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }
}