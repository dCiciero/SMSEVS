using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Entities
{
    public class Election
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        // Private constructor for EF Core
        private Election() { }
        
        public Election(string title, string description, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
                
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date");
                
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = false;
        }
        
        public void Activate()
        {
            if (DateTime.UtcNow > EndDate)
                throw new InvalidOperationException("Cannot activate an election that has already ended");
                
            IsActive = true;
        }
        
        public void Deactivate()
        {
            IsActive = false;
        }
        
        public void Update(string title, string description, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
                
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date");
                
            if (IsActive)
                throw new InvalidOperationException("Cannot update an active election");
                
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public bool IsOngoing()
        {
            var now = DateTime.UtcNow;
            return IsActive && now >= StartDate && now <= EndDate;
        }
    }
}