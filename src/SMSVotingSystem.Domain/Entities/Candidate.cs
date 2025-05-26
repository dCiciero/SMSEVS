using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSVotingSystem.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Party { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }

        public string? ShortCode { get; set; }
        public int ElectionId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted => DeletedAt.HasValue;
        public bool IsRegistered => !IsDeleted && IsActive;



        // Navigation properties
        public virtual Election Election { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }

        // Private constructor for EF Core
        private Candidate() { }

        public Candidate(string name, string shortCode = null, string description = null,
            string email = null, string phoneNumber = null, string party = null, string position = null, int electionId = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));


            // I want shothcode to be generated comprising the first 2 letters from each part of  the name when split by space and the last 3 digits of the phone number
            var nameParts = name.Split(' ');
            shortCode = string.Join("", nameParts.Select(p => p.Substring(0, 2).ToUpperInvariant())) + phoneNumber.Substring(phoneNumber.Length - 3);

            if (string.IsNullOrWhiteSpace(shortCode))
                throw new ArgumentException("Short code cannot be empty", nameof(shortCode));

            ValidateShortCode(shortCode);
            // ShortCode = shortCode.ToUpperInvariant();


            Name = name;
            ShortCode = shortCode.ToUpperInvariant();
            Description = description;
            Email = email;
            PhoneNumber = phoneNumber;
            Party = party;
            Position = position;
            ElectionId = electionId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;

        }


        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name;
            Description = description;
        }

        public void UpdateShortCode(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
                throw new ArgumentException("Short code cannot be empty", nameof(shortCode));

            ValidateShortCode(shortCode);
            ShortCode = shortCode.ToUpperInvariant();
        }

        private void ValidateShortCode(string shortCode)
        {
            if (shortCode.Length > 10)
                throw new ArgumentException("Short code cannot exceed 10 characters", nameof(shortCode));

            if (!shortCode.All(c => char.IsLetterOrDigit(c)))
                throw new ArgumentException("Short code must contain only letters and numbers", nameof(shortCode));
        }
    }
}