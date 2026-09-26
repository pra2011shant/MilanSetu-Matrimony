import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MatchCriteriaItem {
  title: string;
  description: string;
  isMatch: boolean;
  icon?: string;
}

export interface MatchedProfile {
  id: number;
  profileId: string;
  name: string;
  gender: string;
  age: number;
  height: string;
  maritalStatus: string;
  religion: string;
  caste: string;
  motherTongue: string;
  education: string;
  profession: string;
  company?: string;
  annualIncome: string;
  location: string;
  city: string;
  state: string;
  imageUrl: string;
  verified: boolean;
  premium: boolean;
  about?: string;
  matchScore: number;
  matchBadge: string;
  matchSummary: string;
  matchCriteriaList: MatchCriteriaItem[];
  isShortlisted?: boolean;
  interestStatus?: string;
  activityTimestamp?: string;
}

export interface DashboardMatchesResponse {
  totalRecommended: number;
  totalNewToday: number;
  totalVisitors: number;
  totalShortlisted: number;
  totalInterestsReceived: number;
  recommendedMatches: MatchedProfile[];
  newMatches: MatchedProfile[];
  nearMeMatches: MatchedProfile[];
  recentlyViewed: MatchedProfile[];
  shortlistedMatches: MatchedProfile[];
  profileVisitors: MatchedProfile[];
}

@Injectable({
  providedIn: 'root'
})
export class MatchesService {
  private apiUrl = 'http://localhost:5000/api/matches';

  constructor(private http: HttpClient) {}

  getDashboardMatches(): Observable<DashboardMatchesResponse> {
    return this.http.get<DashboardMatchesResponse>(`${this.apiUrl}/dashboard`);
  }

  toggleShortlist(targetUserId: number): Observable<{ success: boolean; isShortlisted: boolean; message: string }> {
    return this.http.post<{ success: boolean; isShortlisted: boolean; message: string }>(
      `${this.apiUrl}/shortlist/${targetUserId}`,
      {}
    );
  }

  recordView(targetUserId: number): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/view/${targetUserId}`, {});
  }

  sendInterest(targetUserId: number, message?: string): Observable<{ success: boolean; status: string; message: string }> {
    return this.http.post<{ success: boolean; status: string; message: string }>(
      `${this.apiUrl}/interest`,
      { targetUserId, message }
    );
  }

  respondToInterest(interestId: number, action: 'Accepted' | 'Declined'): Observable<{ success: boolean; status: string; message: string }> {
    return this.http.put<{ success: boolean; status: string; message: string }>(
      `${this.apiUrl}/interest/${interestId}/respond`,
      { action }
    );
  }
}
