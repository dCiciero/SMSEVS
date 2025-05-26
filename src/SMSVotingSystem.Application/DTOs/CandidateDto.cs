namespace SMSVotingSystem.Application.DTOs
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortCode { get; set; }
        public int ElectionId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CreateCandidateDto
    {
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public int ElectionId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted => DeletedAt.HasValue;
        public bool IsRegistered => !IsDeleted && IsActive;

    }
    
    public class UpdateCandidateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortCode { get; set; }
    }
}