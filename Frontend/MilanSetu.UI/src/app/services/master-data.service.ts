import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';

export interface MasterDataResponse {
  religions: string[];
  motherTongues: string[];
  educations: string[];
  occupations: string[];
  incomeRanges: string[];
  locations: string[];
}

export interface Story {
  id: string | number;
  coupleName: string;
  weddingDate: string;
  location: string;
  imageUrl: string;
  quote: string;
  storySnippet: string;
}

@Injectable({
  providedIn: 'root'
})
export class MasterDataService {
  private apiUrl = 'https://localhost:7047/api';
  private masterDataCache$?: Observable<MasterDataResponse>;

  constructor(private http: HttpClient) {}

  getMasterData(): Observable<MasterDataResponse> {
    if (!this.masterDataCache$) {
      this.masterDataCache$ = this.http.get<MasterDataResponse>(`${this.apiUrl}/masterdata`).pipe(
        shareReplay(1)
      );
    }
    return this.masterDataCache$;
  }

  getSuccessStories(): Observable<Story[]> {
    return this.http.get<Story[]>(`${this.apiUrl}/successstories`);
  }

  getFeaturedProfiles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/home/featured-profiles`);
  }

  getPlatformStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/home/stats`);
  }
}
