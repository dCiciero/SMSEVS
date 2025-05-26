import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';
import { CreateVoterDto, UpdateVoterDto, Voter } from '../models/voter.model';

@Injectable({
  providedIn: 'root'
})
export class VoterService {

  private readonly baseUrl = 'voters';
  
  constructor(private apiService: ApiService) { }
  
  getVoters(): Observable<Voter[]> {
    return this.apiService.get<Voter[]>(this.baseUrl);
  }
  
  getVoter(id: number): Observable<Voter> {
    return this.apiService.get<Voter>(`${this.baseUrl}/${id}`);
  }
  
  createVoter(voterDto: CreateVoterDto): Observable<Voter> {
    return this.apiService.post<Voter, CreateVoterDto>(this.baseUrl, voterDto);
  }
  
  updateVoter(id: number, voterDto: UpdateVoterDto): Observable<Voter> {
    return this.apiService.put<Voter, UpdateVoterDto>(`${this.baseUrl}/${id}`, voterDto);
  }
  
  deleteVoter(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.baseUrl}/${id}`);
  }
}
