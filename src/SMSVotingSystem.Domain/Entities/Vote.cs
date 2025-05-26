using System;

namespace SMSVotingSystem.Domain.Entities
{
    public class Vote
    {
        public int Id { get; private set; }

        // Foreign key properties (MUST be public setter for EF Core)
        public int VoterId { get; set; }
        public int CandidateId { get; set; }
        public int ElectionId { get; set; }

        public DateTime VoteTimestamp { get; private set; }

        // Navigation properties (private set is fine)
        public Voter Voter { get; private set; }
        public Candidate Candidate { get; private set; }
        public Election Election { get; private set; }

        // Private constructor for EF Core
        private Vote() { }

        // Public constructor for your app
        public Vote(int voterId, int candidateId, int electionId)
        {
            VoterId = voterId;
            CandidateId = candidateId;
            ElectionId = electionId;
            VoteTimestamp = DateTime.UtcNow;
        }
    }
}
