using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SMSVotingSystem.Application.Common.Exceptions;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;

namespace SMSVotingSystem.Application.Services
{
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ILogger<ElectionService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(IElectionRepository electionRepository, IUnitOfWork unitOfWork, ICandidateRepository candidateRepository, ILogger<ElectionService> logger)
        {
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
            _candidateRepository = candidateRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ElectionDto>> GetAllElectionsAsync()
        {
            var elections = await _electionRepository.GetAllAsync();
            return elections.Select(e => new ElectionDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsActive = e.IsActive
            });
        }
        
        public async Task<ElectionDto> GetElectionByIdAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                return null;
                
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task<ElectionDto> GetActiveElectionAsync()
        {
            var election = await _electionRepository.GetActiveElectionAsync();
            if (election == null)
                return null;
                
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task<ElectionDto> CreateElectionAsync(CreateElectionDto createElectionDto)
        {
            var election = new Election(
                createElectionDto.Title,
                createElectionDto.Description,
                createElectionDto.StartDate,
                createElectionDto.EndDate);
                
            await _electionRepository.AddAsync(election);
            await _unitOfWork.SaveChangesAsync();
            
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task<ElectionDto> UpdateElectionAsync(int id, UpdateElectionDto updateElectionDto)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new NotFoundException($"Election with ID {id} not found.");
                
            election.Update(
                updateElectionDto.Title,
                updateElectionDto.Description,
                updateElectionDto.StartDate,
                updateElectionDto.EndDate);
                
            await _electionRepository.UpdateAsync(election);
            await _unitOfWork.SaveChangesAsync();
            
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task<ElectionDto> ActivateElectionAsync(int id)
        {
            // First, deactivate any currently active election
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            if (activeElection != null && activeElection.Id != id)
            {
                activeElection.Deactivate();
                await _electionRepository.UpdateAsync(activeElection);
            }
            
            // Now activate the specified election
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new NotFoundException($"Election with ID {id} not found.");
                
            election.Activate();
            await _electionRepository.UpdateAsync(election);
            await _unitOfWork.SaveChangesAsync();
            
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task<ElectionDto> DeactivateElectionAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new NotFoundException($"Election with ID {id} not found.");
                
            election.Deactivate();
            await _electionRepository.UpdateAsync(election);
            await _unitOfWork.SaveChangesAsync();
            
            return new ElectionDto
            {
                Id = election.Id,
                Title = election.Title,
                Description = election.Description,
                StartDate = election.StartDate,
                EndDate = election.EndDate,
                IsActive = election.IsActive
            };
        }
        
        public async Task DeleteElectionAsync(int id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new NotFoundException($"Election with ID {id} not found.");
                
            if (election.IsActive)
                throw new InvalidOperationException("Cannot delete an active election.");
                
            await _electionRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    
        public async Task<IEnumerable<CandidateDto>> GetCandidatesForElectionAsync(int electionId)
        {
            try
            {
                var candidates = await _candidateRepository.GetByElectionIdAsync(electionId);
                if (candidates == null || !candidates.Any())
                {
                    return new List<CandidateDto>();
                }

                return candidates.Select(c => new CandidateDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ShortCode = c.ShortCode,
                    ElectionId = c.ElectionId
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting candidates for election ID {ElectionId}", electionId);
                return new List<CandidateDto>();
            }
        }
    }
    
}