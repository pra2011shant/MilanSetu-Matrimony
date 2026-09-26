import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatchesService, MatchedProfile, DashboardMatchesResponse } from '../../services/matches.service';

type MatchTab = 'recommended' | 'new' | 'nearMe' | 'shortlisted' | 'visitors' | 'recent';

@Component({
  selector: 'app-matches',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './matches.component.html',
  styleUrls: ['./matches.component.css']
})
export class MatchesComponent implements OnInit {
  activeTab: MatchTab = 'recommended';
  isLoading = true;
  dashboardData: DashboardMatchesResponse | null = null;
  selectedProfileForModal: MatchedProfile | null = null;
  selectedScoreProfile: MatchedProfile | null = null;
  toastMessage: string | null = null;

  constructor(private matchesService: MatchesService) {}

  ngOnInit(): void {
    this.loadMatches();
  }

  loadMatches(): void {
    this.isLoading = true;
    this.matchesService.getDashboardMatches().subscribe({
      next: (res) => {
        this.isLoading = false;
        this.dashboardData = res;
      },
      error: () => {
        this.isLoading = false;
        // Fallback demo data
        this.dashboardData = this.getMockDashboardData();
      }
    });
  }

  setTab(tab: MatchTab): void {
    this.activeTab = tab;
  }

  getActiveProfiles(): MatchedProfile[] {
    if (!this.dashboardData) return [];
    switch (this.activeTab) {
      case 'recommended':
        return this.dashboardData.recommendedMatches || [];
      case 'new':
        return this.dashboardData.newMatches || [];
      case 'nearMe':
        return this.dashboardData.nearMeMatches || [];
      case 'shortlisted':
        return this.dashboardData.shortlistedMatches || [];
      case 'visitors':
        return this.dashboardData.profileVisitors || [];
      case 'recent':
        return this.dashboardData.recentlyViewed || [];
      default:
        return [];
    }
  }

  toggleShortlist(profile: MatchedProfile, event: Event): void {
    event.stopPropagation();
    profile.isShortlisted = !profile.isShortlisted;

    this.matchesService.toggleShortlist(profile.id).subscribe({
      next: (res) => {
        profile.isShortlisted = res.isShortlisted;
        this.showToast(res.message);
        if (this.dashboardData) {
          if (res.isShortlisted) {
            if (!this.dashboardData.shortlistedMatches.some(p => p.id === profile.id)) {
              this.dashboardData.shortlistedMatches.push(profile);
            }
          } else {
            this.dashboardData.shortlistedMatches = this.dashboardData.shortlistedMatches.filter(p => p.id !== profile.id);
          }
          this.dashboardData.totalShortlisted = this.dashboardData.shortlistedMatches.length;
        }
      },
      error: () => {
        this.showToast(profile.isShortlisted ? `⭐ Added ${profile.name} to Shortlist` : `Removed from Shortlist`);
      }
    });
  }

  sendInterest(profile: MatchedProfile, event: Event): void {
    event.stopPropagation();
    const willSend = profile.interestStatus !== 'Pending';
    profile.interestStatus = willSend ? 'Pending' : 'None';

    this.matchesService.sendInterest(profile.id).subscribe({
      next: (res) => {
        profile.interestStatus = res.status;
        this.showToast(res.message);
      },
      error: () => {
        this.showToast(willSend ? `✨ Express Interest sent to ${profile.name}!` : `Interest withdrawn.`);
      }
    });
  }

  openMatchScoreModal(profile: MatchedProfile, event: Event): void {
    event.stopPropagation();
    this.selectedScoreProfile = profile;
  }

  closeMatchScoreModal(): void {
    this.selectedScoreProfile = null;
  }

  openProfileDetails(profile: MatchedProfile): void {
    this.selectedProfileForModal = profile;
    this.matchesService.recordView(profile.id).subscribe({
      next: () => {},
      error: () => {}
    });
  }

  closeProfileDetails(): void {
    this.selectedProfileForModal = null;
  }

  showToast(msg: string): void {
    this.toastMessage = msg;
    setTimeout(() => {
      this.toastMessage = null;
    }, 3500);
  }

  private getMockDashboardData(): DashboardMatchesResponse {
    const mockProfiles: MatchedProfile[] = [
      {
        id: 301,
        profileId: "MS-301",
        name: "Ananya Sharma",
        gender: "Bride",
        age: 25,
        height: "5'4\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Brahmin",
        motherTongue: "Hindi",
        education: "B.Tech - Computer Science",
        profession: "Senior Software Engineer",
        company: "Microsoft",
        annualIncome: "₹20 - ₹25 Lakhs",
        location: "Bengaluru, Karnataka",
        city: "Bengaluru",
        state: "Karnataka",
        imageUrl: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Cheerful techie who loves exploring coffee shops, road journeys, and values family connection.",
        matchScore: 96,
        matchBadge: "✨ 96% Top Recommendation",
        matchSummary: "6 of 6 Preferences Matched",
        matchCriteriaList: [
          { title: "Age Range", description: "25 Yrs (Preferred 21-28)", isMatch: true },
          { title: "Religion", description: "Hindu (Exact Match)", isMatch: true },
          { title: "Mother Tongue", description: "Hindi", isMatch: true },
          { title: "Education", description: "B.Tech / Masters", isMatch: true },
          { title: "Location", description: "Bengaluru (Same City)", isMatch: true },
          { title: "Diet", description: "Vegetarian", isMatch: true }
        ],
        isShortlisted: false,
        interestStatus: "None"
      },
      {
        id: 302,
        profileId: "MS-302",
        name: "Dr. Simran Kaur Gill",
        gender: "Bride",
        age: 26,
        height: "5'6\"",
        maritalStatus: "Never Married",
        religion: "Sikh",
        caste: "Jat Sikh",
        motherTongue: "Punjabi",
        education: "M.D. Pediatrics",
        profession: "Resident Doctor",
        company: "Apollo Hospital",
        annualIncome: "₹16 - ₹22 Lakhs",
        location: "Chandigarh / Delhi",
        city: "Chandigarh",
        state: "Punjab",
        imageUrl: "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Dedicated pediatrician with a warm smile. Enjoys classical music and culinary arts.",
        matchScore: 91,
        matchBadge: "✨ 91% High Compatibility",
        matchSummary: "5 of 6 Preferences Matched",
        matchCriteriaList: [
          { title: "Age Range", description: "26 Yrs (Preferred 21-28)", isMatch: true },
          { title: "Education", description: "Doctor / Post Graduate", isMatch: true },
          { title: "Marital Status", description: "Never Married", isMatch: true },
          { title: "Career", description: "Doctor / Healthcare", isMatch: true },
          { title: "Diet", description: "Vegetarian", isMatch: true }
        ],
        isShortlisted: true,
        interestStatus: "Pending"
      },
      {
        id: 303,
        profileId: "MS-303",
        name: "Pooja Banerjee",
        gender: "Bride",
        age: 27,
        height: "5'3\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Bengali Brahmin",
        motherTongue: "Bengali",
        education: "Chartered Accountant (CA)",
        profession: "Finance Manager",
        company: "Deloitte",
        annualIncome: "₹18 - ₹25 Lakhs",
        location: "Bengaluru / Kolkata",
        city: "Bengaluru",
        state: "Karnataka",
        imageUrl: "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Grounded finance specialist with a love for Rabindra Sangeet and travel.",
        matchScore: 85,
        matchBadge: "⭐ 85% Great Match",
        matchSummary: "5 of 6 Preferences Matched",
        matchCriteriaList: [
          { title: "Age Range", description: "27 Yrs", isMatch: true },
          { title: "Religion", description: "Hindu", isMatch: true },
          { title: "Location", description: "Bengaluru", isMatch: true },
          { title: "Education", description: "Chartered Accountant", isMatch: true }
        ],
        isShortlisted: false,
        interestStatus: "None"
      },
      {
        id: 304,
        profileId: "MS-304",
        name: "Meera Nair",
        gender: "Bride",
        age: 26,
        height: "5'5\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Nair",
        motherTongue: "Malayalam",
        education: "MBA - Marketing",
        profession: "Brand Strategist",
        company: "Unilever",
        annualIncome: "₹15 - ₹20 Lakhs",
        location: "Kochi / Mumbai",
        city: "Mumbai",
        state: "Maharashtra",
        imageUrl: "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: false,
        about: "Creative brand strategist who loves reading, contemporary art, and family dinners.",
        matchScore: 80,
        matchBadge: "🤝 80% Good Match",
        matchSummary: "4 of 6 Preferences Matched",
        matchCriteriaList: [
          { title: "Age Range", description: "26 Yrs", isMatch: true },
          { title: "Religion", description: "Hindu", isMatch: true },
          { title: "Marital Status", description: "Never Married", isMatch: true }
        ],
        isShortlisted: false,
        interestStatus: "None"
      }
    ];

    return {
      totalRecommended: 12,
      totalNewToday: 8,
      totalVisitors: 6,
      totalShortlisted: 1,
      totalInterestsReceived: 4,
      recommendedMatches: mockProfiles,
      newMatches: [mockProfiles[0], mockProfiles[2]],
      nearMeMatches: [mockProfiles[0], mockProfiles[2]],
      recentlyViewed: [mockProfiles[1], mockProfiles[3]],
      shortlistedMatches: [mockProfiles[1]],
      profileVisitors: [mockProfiles[0], mockProfiles[3]]
    };
  }
}
