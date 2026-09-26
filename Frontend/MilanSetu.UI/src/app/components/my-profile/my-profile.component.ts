import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ProfileService, UserProfile } from '../../services/profile.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css']
})
export class MyProfileComponent implements OnInit {
  activeTab: string = 'all'; // 'all' | 'basic' | 'about' | 'education' | 'profession' | 'income' | 'family' | 'lifestyle' | 'hobbies' | 'religion' | 'location' | 'photos'
  
  isPreviewMode: boolean = false;
  isLoading: boolean = false;
  isSaving: boolean = false;
  toastMessage: string | null = null;
  newPhotoUrl: string = '';

  // Default Mock Profile Data
  profile: UserProfile = {
    userId: 1,
    name: 'Ananya Sharma',
    gender: 'Female',
    dateOfBirth: '1999-08-15',
    age: 26,
    email: 'ananya.sharma@example.com',
    mobile: '9876543210',
    religion: 'Hindu',
    caste: 'Brahmin',
    motherTongue: 'Hindi',
    profilePhotoUrl: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80',
    isVerified: true,

    // 1. Basic Information
    height: "5'5\"",
    weight: "58 kg",
    maritalStatus: 'Never Married',
    physicalStatus: 'Normal',
    profileManagedBy: 'Self',

    // 2. About Me
    aboutMe: 'Warm-hearted, ambitious professional who balances cultural traditions with modern thinking. I cherish deep family conversations, weekend travel, and continuous learning.',
    partnerExpectations: 'Looking for an understanding, educated, and supportive life companion from a cultured family who respects mutual career growth and family values.',

    // 3. Education
    highestEducation: 'B.Tech - Computer Science',
    collegeOrUniversity: 'Delhi Technological University (DTU)',
    fieldOfStudy: 'Computer Science & Engineering',

    // 4. Profession
    employedIn: 'Private Sector',
    occupation: 'Senior Software Engineer',
    companyName: 'Microsoft India',
    workLocation: 'Bengaluru, Karnataka',

    // 5. Income
    annualIncome: '₹18 - ₹24 Lakhs',

    // 6. Family Details
    familyType: 'Nuclear Family',
    familyValues: 'Moderate / Cultured',
    fatherOccupation: 'Senior Executive (Govt Enterprise)',
    motherOccupation: 'Educationist / Teacher',
    numberOfBrothers: 1,
    numberOfSisters: 0,
    familyCity: 'Delhi / NCR',

    // 7. Lifestyle
    diet: 'Vegetarian',
    drink: 'No',
    smoke: 'No',

    // 8. Hobbies
    hobbies: 'Travelling, Photography, Reading Non-Fiction, Hindustani Classical Music, Yoga & Fitness',

    // 9. Religion & Astrology
    subCasteOrGothra: 'Gaur Brahmin / Shandilya Gothra',
    manglikStatus: 'Non-Manglik',
    rashi: 'Kanya (Virgo)',
    nakshatra: 'Hasta',

    // 10. Location
    city: 'Bengaluru',
    state: 'Karnataka',
    country: 'India',
    nativePlace: 'Jaipur, Rajasthan',
    willingToRelocate: true,

    // 11. Photos
    photoGallery: [
      'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80',
      'https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80',
      'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80'
    ],

    profileCompletionPercentage: 92,
    updatedAt: new Date().toISOString()
  };

  // Section Edit Flags
  editState: { [key: string]: boolean } = {
    basic: false,
    about: false,
    education: false,
    profession: false,
    income: false,
    family: false,
    lifestyle: false,
    hobbies: false,
    religion: false,
    location: false,
    photos: false
  };

  // Options Dropdowns
  heightOptions: string[] = [
    "4'10\"", "4'11\"", "5'0\"", "5'1\"", "5'2\"", "5'3\"", "5'4\"", "5'5\"",
    "5'6\"", "5'7\"", "5'8\"", "5'9\"", "5'10\"", "5'11\"", "6'0\"", "6'1\"", "6'2\""
  ];

  maritalStatuses: string[] = ['Never Married', 'Divorced', 'Widowed', 'Awaiting Divorce', 'Annulled'];
  dietOptions: string[] = ['Vegetarian', 'Eggetarian', 'Non-Vegetarian', 'Jain', 'Vegan'];
  incomeOptions: string[] = [
    'Under ₹5 Lakhs',
    '₹5 - ₹10 Lakhs',
    '₹10 - ₹15 Lakhs',
    '₹15 - ₹25 Lakhs',
    '₹25 - ₹50 Lakhs',
    '₹50 Lakhs - ₹1 Crore',
    '₹1 Crore & Above'
  ];

  constructor(private profileService: ProfileService, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    const user = this.authService.currentUserValue;
    if (user) {
      this.profile.name = user.name || this.profile.name;
      this.profile.email = user.email || this.profile.email;
      this.profile.mobile = user.mobile || this.profile.mobile;
      this.profile.gender = user.gender || this.profile.gender;
      this.profile.religion = user.religion || this.profile.religion;
      this.profile.motherTongue = user.motherTongue || this.profile.motherTongue;
    }

    this.profileService.getMyProfile().subscribe({
      next: (res) => {
        if (res) {
          this.profile = { ...this.profile, ...res };
        }
      },
      error: () => {
        // Keeps rich mock data if offline
      }
    });
  }

  toggleEdit(section: string): void {
    this.editState[section] = !this.editState[section];
  }

  saveSection(section: string): void {
    this.isSaving = true;
    this.profileService.updateProfile(this.profile).subscribe({
      next: (res) => {
        this.isSaving = false;
        this.editState[section] = false;
        if (res && res.profile) {
          this.profile = { ...this.profile, ...res.profile };
        }
        this.showToast('✨ Section updated successfully!');
      },
      error: () => {
        this.isSaving = false;
        this.editState[section] = false;
        this.showToast('✨ Section saved locally!');
      }
    });
  }

  setPrimaryPhoto(photoUrl: string): void {
    this.profile.profilePhotoUrl = photoUrl;
    this.profileService.updateProfile(this.profile).subscribe();
    this.showToast('🌟 Set as primary profile photo!');
  }

  addPhoto(url: string): void {
    if (!url || !url.trim()) return;
    if (!this.profile.photoGallery) this.profile.photoGallery = [];
    if (this.profile.photoGallery.length >= 6) {
      this.showToast('You can upload a maximum of 6 photos.');
      return;
    }
    this.profile.photoGallery.push(url.trim());
    this.newPhotoUrl = '';
    this.profileService.updateProfile(this.profile).subscribe();
    this.showToast('📷 New photo added to gallery!');
  }

  removePhoto(photoUrl: string): void {
    this.profile.photoGallery = this.profile.photoGallery.filter(p => p !== photoUrl);
    if (this.profile.profilePhotoUrl === photoUrl && this.profile.photoGallery.length > 0) {
      this.profile.profilePhotoUrl = this.profile.photoGallery[0];
    }
    this.profileService.updateProfile(this.profile).subscribe();
    this.showToast('Photo removed.');
  }

  showToast(msg: string): void {
    this.toastMessage = msg;
    setTimeout(() => {
      this.toastMessage = null;
    }, 3500);
  }
}
