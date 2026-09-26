import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { AlertService } from '../../services/alert.service';
import { MasterDataService, Story } from '../../services/master-data.service';
import { MatchesService } from '../../services/matches.service';

export interface Profile {
  id: string;
  numericId?: number;
  name: string;
  gender: 'Bride' | 'Groom' | string;
  age: number;
  height: string;
  religion: string;
  caste?: string;
  motherTongue: string;
  education: string;
  profession: string;
  company?: string;
  location: string;
  imageUrl: string;
  verified: boolean;
  premium: boolean;
  about: string;
  interestSent?: boolean;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  // Search Form State
  searchCriteria = {
    lookingFor: 'Bride',
    minAge: 22,
    maxAge: 30,
    religion: 'All Religions',
    motherTongue: 'All Languages'
  };

  // Filter list options (Populated dynamically from database)
  religions: string[] = ['All Religions'];
  motherTongues: string[] = ['All Languages'];
  ageOptions: number[] = Array.from({ length: 43 }, (_, i) => 18 + i); // 18 to 60

  activeTab: 'all' | 'brides' | 'grooms' | 'premium' = 'all';

  // Selected Profile for Modal
  selectedProfile: Profile | null = null;
  toastMessage: string | null = null;

  // Dynamic Profiles & Stories from Database
  allProfiles: Profile[] = [];
  filteredProfiles: Profile[] = [];
  successStories: Story[] = [];
  stats: any = {
    verifiedProfiles: '50,000+',
    happyMarriages: '12,500+',
    matchAccuracy: '98%',
    communities: '100+'
  };

  constructor(
    private masterDataService: MasterDataService,
    private matchesService: MatchesService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.loadMasterData();
    this.loadFeaturedProfiles();
    this.loadSuccessStories();
  }

  loadMasterData(): void {
    this.masterDataService.getMasterData().subscribe({
      next: (data) => {
        if (data) {
          this.religions = ['All Religions', ...(data.religions || [])];
          this.motherTongues = ['All Languages', ...(data.motherTongues || [])];
        }
      },
      error: () => {}
    });

    this.masterDataService.getPlatformStats().subscribe({
      next: (res) => {
        if (res) this.stats = res;
      },
      error: () => {}
    });
  }

  loadFeaturedProfiles(): void {
    this.masterDataService.getFeaturedProfiles().subscribe({
      next: (profiles) => {
        this.allProfiles = profiles || [];
        this.filteredProfiles = [...this.allProfiles];
      },
      error: () => {
        this.allProfiles = [];
        this.filteredProfiles = [];
      }
    });
  }

  loadSuccessStories(): void {
    this.masterDataService.getSuccessStories().subscribe({
      next: (stories) => {
        this.successStories = stories || [];
      },
      error: () => {
        this.successStories = [];
      }
    });
  }

  onSearch() {
    this.filteredProfiles = this.allProfiles.filter(p => {
      const matchGender = this.searchCriteria.lookingFor === 'Bride' ? p.gender === 'Bride' || p.gender === 'Female' : p.gender === 'Groom' || p.gender === 'Male';
      const matchAge = p.age >= this.searchCriteria.minAge && p.age <= this.searchCriteria.maxAge;
      const matchReligion = this.searchCriteria.religion === 'All Religions' || p.religion.toLowerCase() === this.searchCriteria.religion.toLowerCase();
      const matchLanguage = this.searchCriteria.motherTongue === 'All Languages' || p.motherTongue.toLowerCase() === this.searchCriteria.motherTongue.toLowerCase();

      return matchGender && matchAge && matchReligion && matchLanguage;
    });

    const element = document.getElementById('featured');
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }

    this.alertService.toastSuccess(`Found ${this.filteredProfiles.length} compatible profile(s)!`, 'Search Results');
  }

  applyTabFilter(tab: 'all' | 'brides' | 'grooms' | 'premium') {
    this.activeTab = tab;
    if (tab === 'all') {
      this.filteredProfiles = [...this.allProfiles];
    } else if (tab === 'brides') {
      this.filteredProfiles = this.allProfiles.filter(p => p.gender === 'Bride' || p.gender === 'Female');
    } else if (tab === 'grooms') {
      this.filteredProfiles = this.allProfiles.filter(p => p.gender === 'Groom' || p.gender === 'Male');
    } else if (tab === 'premium') {
      this.filteredProfiles = this.allProfiles.filter(p => p.premium);
    }
  }

  sendInterest(profile: Profile, event: Event) {
    event.stopPropagation();
    profile.interestSent = !profile.interestSent;
    if (profile.numericId) {
      this.matchesService.sendInterest(profile.numericId).subscribe();
    }
    if (profile.interestSent) {
      this.alertService.toastSuccess(`Expressed Interest in ${profile.name}! Notification sent.`, 'Express Interest');
    } else {
      this.alertService.toastInfo(`Interest in ${profile.name} withdrawn.`, 'Express Interest');
    }
  }

  openProfileModal(profile: Profile) {
    this.selectedProfile = profile;
    if (profile.numericId) {
      this.matchesService.recordView(profile.numericId).subscribe();
    }
  }

  closeProfileModal() {
    this.selectedProfile = null;
  }
}
