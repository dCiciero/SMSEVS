using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.DTOs
{
    public class VoterDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastVoted { get; set; }
    }
    
    public class CreateVoterDto
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
    }
    
    public class UpdateVoterDto
    {
        public string Name { get; set; }
        public bool IsRegistered { get; set; }
    }
}