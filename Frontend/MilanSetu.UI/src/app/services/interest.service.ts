import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface InterestItem {
  id: number;
  userId: number;
  profileId: string;
  name: string;
  gender: string;
  age: number;
  height: string;
  religion: string;
  caste: string;
  motherTongue: string;
  education: string;
  profession: string;
  annualIncome: string;
  location: string;
  imageUrl: string;
  verified: boolean;
  status: 'Pending' | 'Accepted' | 'Declined' | 'Withdrawn';
  customMessage?: string;
  sentAt: string;
  respondedAt?: string;
  matchScore: number;
  isCommunicationUnlocked: boolean;
  contactMobile?: string;
  contactEmail?: string;
  preferredCallTime?: string;
}

export interface InterestCounts {
  pendingReceived: number;
  acceptedConnected: number;
  pendingSent: number;
  totalReceived: number;
}

@Injectable({
  providedIn: 'root'
})
export class InterestService {
  private apiUrl = 'http://localhost:5000/api/interests';

  constructor(private http: HttpClient) {}

  getInterestCounts(): Observable<InterestCounts> {
    return this.http.get<InterestCounts>(`${this.apiUrl}/counts`);
  }

  getReceivedInterests(status: string = 'All'): Observable<InterestItem[]> {
    return this.http.get<InterestItem[]>(`${this.apiUrl}/received?status=${status}`);
  }

  getSentInterests(status: string = 'All'): Observable<InterestItem[]> {
    return this.http.get<InterestItem[]>(`${this.apiUrl}/sent?status=${status}`);
  }

  getConnectedMatches(): Observable<InterestItem[]> {
    return this.http.get<InterestItem[]>(`${this.apiUrl}/connected`);
  }

  sendInterest(receiverUserId: number, customMessage?: string): Observable<{ success: boolean; interestId: number; status: string; message: string }> {
    return this.http.post<{ success: boolean; interestId: number; status: string; message: string }>(
      `${this.apiUrl}/send`,
      { receiverUserId, customMessage }
    );
  }

  respondToInterest(id: number, action: 'Accepted' | 'Declined'): Observable<{ success: boolean; status: string; isCommunicationUnlocked: boolean; message: string }> {
    return this.http.put<{ success: boolean; status: string; isCommunicationUnlocked: boolean; message: string }>(
      `${this.apiUrl}/${id}/respond`,
      { action }
    );
  }

  withdrawInterest(id: number): Observable<{ success: boolean; message: string }> {
    return this.http.delete<{ success: boolean; message: string }>(`${this.apiUrl}/${id}/withdraw`);
  }
}
