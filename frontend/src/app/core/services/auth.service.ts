import { Injectable, effect, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, catchError, map, of, tap } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { ApiService } from './api.service';
import { AuthResponse, LoginRequest, RegisterRequest, SessionUser } from '../models/auth.models';
import { UserProfile } from '../models/user.model';

interface AuthState {
  user: SessionUser | null;
  profile: UserProfile | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);
  private readonly tokenStorageKey = 'roomiematch.accessToken';
  private readonly refreshStorageKey = 'roomiematch.refreshToken';

  private readonly state = signal<AuthState>({ user: null, profile: null });
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

  private readonly userSignal = computed(() => this.state().profile);
  private readonly sessionSignal = computed(() => this.state().user);
  readonly user$ = toObservable(this.userSignal);
  readonly session$ = toObservable(this.sessionSignal);
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor() {
    const storedToken = this.getStoredToken();
    if (storedToken) {
      const session = this.decodeToken(storedToken);
      if (session) {
        this.state.update((current) => ({ ...current, user: session }));
        this.isAuthenticatedSubject.next(true);
        this.loadProfile().subscribe();
      }
    }

    effect(() => {
      const session = this.state().user;
      if (!session) {
        this.isAuthenticatedSubject.next(false);
        return;
      }
      this.isAuthenticatedSubject.next(true);
    });
  }

  login(payload: LoginRequest): Observable<SessionUser | null> {
    return this.api.post<AuthResponse>('auth/login', payload).pipe(
      map((response) => this.handleAuthResponse(response)),
      catchError(() => {
        return of(null);
      })
    );
  }

  register(payload: RegisterRequest): Observable<SessionUser | null> {
    return this.api.post<AuthResponse>('auth/register', payload).pipe(
      map((response) => this.handleAuthResponse(response)),
      catchError(() => of(null))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenStorageKey);
    localStorage.removeItem(this.refreshStorageKey);
    this.state.set({ user: null, profile: null });
    this.router.navigate(['/']);
  }

  currentUser(): SessionUser | null {
    return this.state().user;
  }

  accessToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  refreshProfile(): Observable<UserProfile | null> {
    return this.loadProfile();
  }

  private loadProfile(): Observable<UserProfile | null> {
    return this.api.get<UserProfile>('users/me').pipe(
      tap((profile) => this.state.update((current) => ({ ...current, profile }))),
      catchError(() => {
        this.state.update((current) => ({ ...current, profile: null }));
        return of(null);
      })
    );
  }

  private handleAuthResponse(response: AuthResponse): SessionUser | null {
    localStorage.setItem(this.tokenStorageKey, response.accessToken);
    localStorage.setItem(this.refreshStorageKey, response.refreshToken);
    const session = this.decodeToken(response.accessToken, response.expiresAt);
    if (session) {
      this.state.update((current) => ({ ...current, user: session }));
      this.loadProfile().subscribe();
      return session;
    }
    return null;
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  private decodeToken(token: string, expiresAt?: string): SessionUser | null {
    try {
      const [, payload] = token.split('.');
      const decoded = JSON.parse(atob(payload));
      const expiry = expiresAt ? new Date(expiresAt) : new Date(decoded['exp'] * 1000);
      return {
        id: decoded['sub'],
        email: decoded['email'] ?? '',
        displayName: decoded['displayName'] ?? '',
        role: decoded['role'] ?? decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
        expiresAt: expiry
      } as SessionUser;
    } catch (error) {
      console.error('Failed to decode token', error);
      return null;
    }
  }
}
