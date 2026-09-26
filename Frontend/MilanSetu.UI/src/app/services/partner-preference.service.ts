import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface PartnerPreference {
  userId?: number;
  minAge: number;
  maxAge: number;
  minHeight: string;
  maxHeight: string;
  religion: string;
  community: string;
  motherTongue: string;
  education: string;
  profession: string;
  minAnnualIncome: string;
  preferredLocation: string;
  maritalStatus: string;
  diet: string;
  drink: string;
  smoke: string;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PartnerPreferenceService {
  private apiUrl = 'https://localhost:7047/api/partnerpreference';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getPreferences(): Observable<PartnerPreference> {
    return this.http.get<PartnerPreference>(this.apiUrl, {
      headers: this.getHeaders()
    });
  }

  savePreferences(pref: PartnerPreference): Observable<any> {
    return this.http.put<any>(this.apiUrl, pref, {
      headers: this.getHeaders()
    });
  }
}
