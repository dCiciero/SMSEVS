import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Candidate, CreateCandidateDto, UpdateCandidateDto } from '../models/candidate.model';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {

  private readonly baseUrl = 'candidates';
  
  constructor(private apiService: ApiService) { }
  
  getCandidates(): Observable<Candidate[]> {
    return this.apiService.get<Candidate[]>(this.baseUrl);
  }
  
  getCandidate(id: number): Observable<Candidate> {
    return this.apiService.get<Candidate>(`${this.baseUrl}/${id}`);
  }
  
  createCandidate(candidateDto: CreateCandidateDto): Observable<Candidate> {
    return this.apiService.post<Candidate, CreateCandidateDto>(this.baseUrl, candidateDto);
  }
  
  updateCandidate(id: number, candidateDto: UpdateCandidateDto): Observable<Candidate> {
    return this.apiService.put<Candidate, UpdateCandidateDto>(`${this.baseUrl}/${id}`, candidateDto);
  }
  
  deleteCandidate(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.baseUrl}/${id}`);
  }
}
