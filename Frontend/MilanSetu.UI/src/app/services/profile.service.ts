import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface UserProfile {
  userId: number;
  name: string;
  gender: string;
  dateOfBirth: string;
  age: number;
  email: string;
  mobile: string;
  religion: string;
  caste?: string;
  motherTongue: string;
  profilePhotoUrl?: string;
  isVerified: boolean;

  // 1. Basic Information
  height?: string;
  weight?: string;
  maritalStatus?: string;
  physicalStatus?: string;
  profileManagedBy?: string;

  // 2. About Me
  aboutMe?: string;
  partnerExpectations?: string;

  // 3. Education
  highestEducation?: string;
  collegeOrUniversity?: string;
  fieldOfStudy?: string;

  // 4. Profession
  employedIn?: string;
  occupation?: string;
  companyName?: string;
  workLocation?: string;

  // 5. Income
  annualIncome?: string;

  // 6. Family Details
  familyType?: string;
  familyValues?: string;
  fatherOccupation?: string;
  motherOccupation?: string;
  numberOfBrothers: number;
  numberOfSisters: number;
  familyCity?: string;

  // 7. Lifestyle
  diet?: string;
  drink?: string;
  smoke?: string;

  // 8. Hobbies
  hobbies?: string;

  // 9. Religion & Astrology
  subCasteOrGothra?: string;
  manglikStatus?: string;
  rashi?: string;
  nakshatra?: string;

  // 10. Location
  city?: string;
  state?: string;
  country?: string;
  nativePlace?: string;
  willingToRelocate: boolean;

  // 11. Photos
  photoGallery: string[];

  profileCompletionPercentage: number;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = 'https://localhost:7047/api/profile';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getMyProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/my-profile`, {
      headers: this.getHeaders()
    });
  }

  updateProfile(profile: UserProfile): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, profile, {
      headers: this.getHeaders()
    });
  }
}
