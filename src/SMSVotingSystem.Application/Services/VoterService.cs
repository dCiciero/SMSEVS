using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMSVotingSystem.Application.Common.Exceptions;
using SMSVotingSystem.Application.DTOs;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Domain.Entities;
using SMSVotingSystem.Domain.Repositories;

namespace SMSVotingSystem.Application.Services
{
    public class VoterService : IVoterService
    {
        private readonly IVoterRepository _voterRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public VoterService(IVoterRepository voterRepository, IUnitOfWork unitOfWork)
        {
            _voterRepository = voterRepository;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<VoterDto>> GetAllVotersAsync()
        {
            var voters = await _voterRepository.GetAllAsync();
            return voters.Select(v => new VoterDto
            {
                Id = v.Id,
                PhoneNumber = v.PhoneNumber,
                Name = v.Name,
                IsRegistered = v.IsRegistered,
                RegistrationDate = v.RegistrationDate,
                LastVoted = v.LastVoted
            });
        }
        
        public async Task<VoterDto> GetVoterByIdAsync(int id)
        {
            var voter = await _voterRepository.GetByIdAsync(id);
            if (voter == null)
                return null;
                
            return new VoterDto
            {
                Id = voter.Id,
                PhoneNumber = voter.PhoneNumber,
                Name = voter.Name,
                IsRegistered = voter.IsRegistered,
                RegistrationDate = voter.RegistrationDate,
                LastVoted = voter.LastVoted
            };
        }
        
        public async Task<VoterDto> GetVoterByPhoneNumberAsync(string phoneNumber)
        {
            var voter = await _voterRepository.GetByPhoneNumberAsync(phoneNumber);
            if (voter == null)
                return null;
                
            return new VoterDto
            {
                Id = voter.Id,
                PhoneNumber = voter.PhoneNumber,
                Name = voter.Name,
                IsRegistered = voter.IsRegistered,
                RegistrationDate = voter.RegistrationDate,
                LastVoted = voter.LastVoted
            };
        }
        
        public async Task<VoterDto> RegisterVoterAsync(CreateVoterDto createVoterDto)
        {
            if (await _voterRepository.PhoneNumberExistsAsync(createVoterDto.PhoneNumber))
            {
                throw new ApplicationException("A voter with this phone number already exists.");
            }
            
            var voter = new Voter(createVoterDto.PhoneNumber, createVoterDto.IdNumber, createVoterDto.Name);
            
            await _voterRepository.AddAsync(voter);
            await _unitOfWork.SaveChangesAsync();
            
            return new VoterDto
            {
                Id = voter.Id,
                PhoneNumber = voter.PhoneNumber,
                Name = voter.Name,
                IsRegistered = voter.IsRegistered,
                RegistrationDate = voter.RegistrationDate,
                LastVoted = voter.LastVoted
            };
        }
        
        public async Task<VoterDto> UpdateVoterAsync(int id, UpdateVoterDto updateVoterDto)
        {
            var voter = await _voterRepository.GetByIdAsync(id);
            if (voter == null)
                throw new NotFoundException($"Voter with ID {id} not found.");
                
            voter.UpdateName(updateVoterDto.Name);
            
            if (updateVoterDto.IsRegistered && !voter.IsRegistered)
                voter.Activate();
            else if (!updateVoterDto.IsRegistered && voter.IsRegistered)
                voter.Deactivate();
                
            await _voterRepository.UpdateAsync(voter);
            await _unitOfWork.SaveChangesAsync();
            
            return new VoterDto
            {
                Id = voter.Id,
                PhoneNumber = voter.PhoneNumber,
                Name = voter.Name,
                IsRegistered = voter.IsRegistered,
                RegistrationDate = voter.RegistrationDate,
                LastVoted = voter.LastVoted
            };
        }
        
        public async Task DeleteVoterAsync(int id)
        {
            var voter = await _voterRepository.GetByIdAsync(id);
            if (voter == null)
                throw new NotFoundException($"Voter with ID {id} not found.");
                
            voter.Deactivate();
            await _voterRepository.UpdateAsync(voter);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    
    
    
    
}