import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

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

export interface RegisterResponse {
  message: string;
  user?: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7047/api/auth'; // Default ASP.NET Core HTTPS port or fallback

  constructor(private http: HttpClient) {}

  register(data: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, data).pipe(
      catchError((error) => {
        // Return structured error
        throw error;
      })
    );
  }
}
