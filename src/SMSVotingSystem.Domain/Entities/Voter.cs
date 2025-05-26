using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Entities
{
    public class Voter
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string IdNumber { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastVoted { get; set; }

        public bool HasVoted { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<Vote> Votes { get; set; }
        
        // Private constructor for EF Core
        private Voter() { }
        
        public Voter(string phoneNumber, string idNumber, string name)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
                
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(idNumber))
                throw new ArgumentException("ID number cannot be empty", nameof(idNumber));
            
            PhoneNumber = phoneNumber;
            Name = name;
            IdNumber = idNumber;
            IsRegistered = true;
            RegistrationDate = DateTime.UtcNow;
            HasVoted = false;
            IsActive = true; 
        }
        
        public void UpdateLastVoted()
        {
            LastVoted = DateTime.UtcNow;
        }
        
        public void Deactivate()
        {
            IsRegistered = false;
            IsActive = false;
        }
        
        public void Activate()
        {
            IsRegistered = true;
            IsActive = true;
        }
        
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
                
            Name = name;
        }
    }
}