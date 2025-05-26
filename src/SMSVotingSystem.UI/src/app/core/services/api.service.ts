import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private baseUrl = environment.baseUrl;

  constructor(private http: HttpClient) { }

  // Generic GET method
  get<T>(path: string, params: HttpParams = new HttpParams()): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${path}`, { params });
  }

  // Generic POST method
  post<T, D>(path: string, data: D): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${path}`, data);
  }

  // Generic PUT method
  put<T, D>(path: string, data: D): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${path}`, data);
  }

  // Generic DELETE method
  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${path}`);
  }
}
