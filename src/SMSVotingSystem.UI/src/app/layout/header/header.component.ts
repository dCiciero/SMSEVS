import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { User } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  // isAuthenticated$: Observable<boolean>;
  // currentUser$: Observable<User | null>;
  // isAdmin$: Observable<boolean>;

  constructor(private authService: AuthService, private router: Router) {
    // this.isAuthenticated$ = this.authService.currentUser$.pipe(
    //   map(user => !!user)
    // );
    
    // this.currentUser$ = this.authService.currentUser$.pipe(
    //   map(authResponse => authResponse?.user || null)
    // );
    
    // this.isAdmin$ = this.authService.currentUser$.pipe(
    //   map(authResponse => authResponse?.user?.roles?.includes('Admin') || false)
    // );
  }

  ngOnInit(): void {}

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}