import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Election, CreateElectionDto, UpdateElectionDto } from '../models/election.model';

@Injectable({
  providedIn: 'root'
})
export class ElectionService {

  private readonly baseUrl = 'elections';
  
  constructor(private apiService: ApiService) { }
  
  getElections(): Observable<Election[]> {
    return this.apiService.get<Election[]>(this.baseUrl);
  }
  
  getElection(id: number): Observable<Election> {
    return this.apiService.get<Election>(`${this.baseUrl}/${id}`);
  }
  
  getActiveElection(): Observable<Election> {
    return this.apiService.get<Election>(`${this.baseUrl}/active`);
  }
  
  createElection(electionDto: CreateElectionDto): Observable<Election> {
    return this.apiService.post<Election, CreateElectionDto>(this.baseUrl, electionDto);
  }
  
  updateElection(id: number, electionDto: UpdateElectionDto): Observable<Election> {
    return this.apiService.put<Election, UpdateElectionDto>(`${this.baseUrl}/${id}`, electionDto);
  }
  
  activateElection(id: number): Observable<Election> {
    return this.apiService.put<Election, any>(`${this.baseUrl}/${id}/activate`, {});
  }
  
  deactivateElection(id: number): Observable<Election> {
    return this.apiService.put<Election, any>(`${this.baseUrl}/${id}/deactivate`, {});
  }
  
  deleteElection(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.baseUrl}/${id}`);
  }
}
