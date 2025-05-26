using Microsoft.EntityFrameworkCore;
using SMSVotingSystem.Domain.Entities;

namespace SMSVotingSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<SmsLog> SmsLogs { get; set; }
        public DbSet<TOTPSecret> TOTPSecrets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureVoter(modelBuilder);
            ConfigureCandidate(modelBuilder);
            ConfigureVote(modelBuilder);
            ConfigureElection(modelBuilder);
            ConfigureTOTPSecret(modelBuilder);
            ConfigureSmsLog(modelBuilder);
        }

        private static void ConfigureVoter(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voter>(entity =>
            {
                entity.HasIndex(v => v.PhoneNumber)
                      .IsUnique();
            });
        }

        private static void ConfigureCandidate(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasIndex(c => c.ShortCode)
                      .IsUnique();
            });
        }

        private static void ConfigureVote(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasIndex(v => new { v.VoterId, v.ElectionId })
                      .IsUnique();

                entity.HasOne(v => v.Voter)
                      .WithMany(v => v.Votes)
                      .HasForeignKey(v => v.VoterId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Candidate)
                      .WithMany(v => v.Votes)
                      .HasForeignKey(v => v.CandidateId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Election)
                      .WithMany()
                      .HasForeignKey(v => v.ElectionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureTOTPSecret(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TOTPSecret>(entity =>
            {
                entity.HasIndex(t => t.PhoneNumber)
                      .IsUnique();

                entity.Property(t => t.EncryptedSecretKey)
                      .IsRequired();
            });
        }

        private static void ConfigureElection(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Election>(entity =>
            {
                // Add election-specific configurations here if needed
            });
        }

        private static void ConfigureSmsLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SmsLog>(entity =>
            {
                // Add SMS log-specific configurations here if needed
            });
        }
    }
}
