import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { SmsLog } from '../models/sms-log.model';

@Injectable({
  providedIn: 'root'
})
export class SmsLogService {

  private readonly baseUrl = 'sms';
  
  constructor(private apiService: ApiService) { }
  
  getLogs(): Observable<SmsLog[]> {
    return this.apiService.get<SmsLog[]>(`${this.baseUrl}/logs`);
  }
  
  getRecentLogs(count: number): Observable<SmsLog[]> {
    return this.apiService.get<SmsLog[]>(`${this.baseUrl}/logs/recent/${count}`);
  }
}
