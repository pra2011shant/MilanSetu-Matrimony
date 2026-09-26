import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SearchFilter {
  gender?: string;
  minAge?: number;
  maxAge?: number;
  minHeight?: string;
  maxHeight?: string;
  religion?: string;
  community?: string;
  motherTongue?: string;
  city?: string;
  state?: string;
  country?: string;
  education?: string;
  profession?: string;
  maritalStatus?: string;
  minIncome?: string;
  profileId?: string;
  photoOnly?: boolean;
  verifiedOnly?: boolean;
  sortBy?: string;
}

export interface SearchResultItem {
  id: number;
  profileId: string;
  name: string;
  gender: string;
  age: number;
  height: string;
  maritalStatus: string;
  religion: string;
  caste?: string;
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
  about: string;
  interestSent?: boolean;
  isShortlisted?: boolean;
}

export interface SearchResponse {
  total: number;
  profiles: SearchResultItem[];
}

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private apiUrl = 'https://localhost:7047/api/search';

  constructor(private http: HttpClient) {}

  search(filter: SearchFilter): Observable<SearchResponse> {
    return this.http.post<SearchResponse>(this.apiUrl, filter);
  }
}
