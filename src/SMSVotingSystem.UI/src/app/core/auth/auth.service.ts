import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { AuthResponse, LoginDto, RegisterDto, User } from './auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private apiUrl = environment.baseUrl;

  constructor(private http: HttpClient) {
    // Check if user is stored in local storage
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  login(loginDto: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, loginDto)
      .pipe(
        tap(response => {
          this.setCurrentUser(response);
        })
      );
  }

  register(registerDto: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, registerDto)
      .pipe(
        tap(response => {
          this.setCurrentUser(response);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    const currentUser = this.currentUserSubject.value;
    return currentUser ? currentUser.token : null;
  }

  isAuthenticated(): boolean {
    return !!this.currentUserSubject.value?.token;
  }

  hasRole(role: string): boolean {
    const currentUser = this.currentUserSubject.value;
    return currentUser?.user?.roles?.includes(role) || false;
  }

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  private setCurrentUser(authResponse: AuthResponse): void {
    localStorage.setItem('currentUser', JSON.stringify(authResponse));
    this.currentUserSubject.next(authResponse);
  }
}
