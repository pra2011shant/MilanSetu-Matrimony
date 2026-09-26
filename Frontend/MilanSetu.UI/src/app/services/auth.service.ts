import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface RegisterRequest {
  name: string;
  gender: string;
  dateOfBirth: string;
  email: string;
  mobile: string;
  password: string;
  religion: string;
  caste?: string;
  motherTongue: string;
  location: string;
}

export interface LoginRequest {
  identifier: string; // Email or Mobile
  password: string;
}

export interface ForgotPasswordRequest {
  identifier: string;
}

export interface VerifyOtpRequest {
  identifier: string;
  otpCode: string;
}

export interface ResetPasswordRequest {
  identifier: string;
  otpCode: string;
  newPassword: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  message: string;
  user: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7047/api/auth';
  private currentUserSubject = new BehaviorSubject<any>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  public get currentUserValue(): any {
    return this.currentUserSubject.value;
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data).pipe(
      tap((res) => {
        if (res && res.token) {
          this.setSession(res);
        }
      })
    );
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((res) => {
        if (res && res.token) {
          this.setSession(res);
        }
      })
    );
  }

  googleLogin(googleData: { email: string; name: string; photoUrl?: string; googleId?: string; token?: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/google-login`, googleData).pipe(
      tap((res) => {
        if (res && res.token) {
          this.setSession(res);
        }
      })
    );
  }

  forgotPassword(identifier: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/forgot-password`, { identifier });
  }

  verifyOtp(identifier: string, otpCode: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/verify-otp`, { identifier, otpCode });
  }

  resetPassword(data: ResetPasswordRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset-password`, data);
  }

  logout(): void {
    localStorage.removeItem('ms_jwt_token');
    localStorage.removeItem('ms_user_profile');
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem('ms_jwt_token');
  }

  private setSession(authResult: AuthResponse): void {
    localStorage.setItem('ms_jwt_token', authResult.token);
    localStorage.setItem('ms_user_profile', JSON.stringify(authResult.user));
    this.currentUserSubject.next(authResult.user);
  }

  private getUserFromStorage(): any {
    const userStr = localStorage.getItem('ms_user_profile');
    return userStr ? JSON.parse(userStr) : null;
  }
}
