import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AdminStats {
  totalUsers: number;
  verifiedUsers: number;
  pendingVerifications: number;
  maleUsers: number;
  femaleUsers: number;
  totalInterests: number;
  acceptedInterests: number;
  totalMessages: number;
  totalStories: number;
  recentUsers: any[];
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'http://localhost:5000/api/admin';

  constructor(private http: HttpClient) {}

  getDashboardStats(): Observable<AdminStats> {
    return this.http.get<AdminStats>(`${this.apiUrl}/dashboard-stats`);
  }

  getUsers(params?: { search?: string; religion?: string; gender?: string; isVerified?: boolean }): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/users`, { params: params as any });
  }

  toggleVerifyUser(userId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/users/${userId}/toggle-verify`, {});
  }

  toggleBlockUser(userId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/users/${userId}/toggle-block`, {});
  }

  deleteUser(userId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/users/${userId}`);
  }

  getStories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/stories`);
  }

  createStory(story: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/stories`, story);
  }

  deleteStory(storyId: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/stories/${storyId}`);
  }

  addReligion(name: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/master-data/religion`, { name, isActive: true });
  }

  addMotherTongue(name: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/master-data/mothertongue`, { name, isActive: true });
  }

  addLocation(cityName: string, stateName: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/master-data/location`, { cityName, stateName, isPopular: true, country: 'India' });
  }
}
