
using SMSVotingSystem.Application.Common.Exceptions;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;

namespace SMSVotingSystem.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public CandidateService(ICandidateRepository candidateRepository, IUnitOfWork unitOfWork)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync()
        {
            var candidates = await _candidateRepository.GetAllAsync();
            return candidates.Select(c => new CandidateDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ShortCode = c.ShortCode,
                Party = c.Party,
                Position = c.Position,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
            });
        }
        
        public async Task<CandidateDto> GetCandidateByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                return null;
                
            return new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Description = candidate.Description,
                ShortCode = candidate.ShortCode
            };
        }
        
        public async Task<CandidateDto> GetCandidateByShortCodeAsync(string shortCode)
        {
            var candidate = await _candidateRepository.GetByShortCodeAsync(shortCode);
            if (candidate == null)
                return null;
                
            return new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Description = candidate.Description,
                ShortCode = candidate.ShortCode
            };
        }
        
        public async Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto)
        {
            if (await _candidateRepository.ShortCodeExistsAsync(createCandidateDto.ShortCode))
            {
                throw new ApplicationException("A candidate with this short code already exists.");
            }
            
            var candidate = new Candidate(
                createCandidateDto.Name,
                createCandidateDto.ShortCode,
                createCandidateDto.Description,
                createCandidateDto.Email,
                createCandidateDto.PhoneNumber,
                createCandidateDto.Party,
                createCandidateDto.Position,
                createCandidateDto.ElectionId
                );
                
            await _candidateRepository.AddAsync(candidate);
            await _unitOfWork.SaveChangesAsync();
            
            return new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Description = candidate.Description,
                ShortCode = candidate.ShortCode,
                Party = candidate.Party,
                Position = candidate.Position,
            };
        }
        
        public async Task<CandidateDto> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException($"Candidate with ID {id} not found.");
                
            // Update candidate name and description
            candidate.Update(updateCandidateDto.Name, updateCandidateDto.Description);
            
            // Update short code if provided and different
            if (!string.IsNullOrEmpty(updateCandidateDto.ShortCode) && 
                !candidate.ShortCode.Equals(updateCandidateDto.ShortCode, StringComparison.OrdinalIgnoreCase))
            {
                if (await _candidateRepository.ShortCodeExistsAsync(updateCandidateDto.ShortCode))
                {
                    throw new ApplicationException("A candidate with this short code already exists.");
                }
                
                candidate.UpdateShortCode(updateCandidateDto.ShortCode);
            }
            
            await _candidateRepository.UpdateAsync(candidate);
            await _unitOfWork.SaveChangesAsync();
            
            return new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Description = candidate.Description,
                ShortCode = candidate.ShortCode
            };
        }
        
        public async Task DeleteCandidateAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id);
            if (candidate == null)
                throw new NotFoundException($"Candidate with ID {id} not found.");
                
            await _candidateRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    
    
}