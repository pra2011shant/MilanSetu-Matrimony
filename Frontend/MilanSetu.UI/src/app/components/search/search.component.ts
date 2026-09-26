import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { SearchService, SearchFilter, SearchResultItem } from '../../services/search.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  filter: SearchFilter = {
    gender: 'Bride',
    minAge: 21,
    maxAge: 32,
    religion: 'All Religions',
    community: '',
    motherTongue: 'All Languages',
    city: '',
    state: '',
    country: 'India',
    education: 'All Education Levels',
    profession: 'All Professions',
    maritalStatus: 'All Marital Statuses',
    minHeight: "5'0\"",
    maxHeight: "6'0\"",
    minIncome: 'No Bar',
    profileId: '',
    photoOnly: false,
    verifiedOnly: false,
    sortBy: 'relevance'
  };

  viewMode: 'grid' | 'list' = 'grid';
  isLoading = false;
  profiles: SearchResultItem[] = [];
  totalMatches = 0;
  selectedProfile: SearchResultItem | null = null;
  toastMessage: string | null = null;

  // Dropdown Lists
  ageOptions: number[] = Array.from({ length: 43 }, (_, i) => 18 + i); // 18 to 60
  
  heightOptions: string[] = [
    "4'10\"", "4'11\"", "5'0\"", "5'1\"", "5'2\"", "5'3\"", "5'4\"", "5'5\"",
    "5'6\"", "5'7\"", "5'8\"", "5'9\"", "5'10\"", "5'11\"", "6'0\"", "6'1\"", "6'2\"", "6'3\"", "6'4\"", "6'5\""
  ];

  religions: string[] = [
    'All Religions',
    'Hindu',
    'Muslim',
    'Sikh',
    'Christian',
    'Jain',
    'Buddhist',
    'Parsi'
  ];

  motherTongues: string[] = [
    'All Languages',
    'Hindi',
    'Punjabi',
    'Bengali',
    'Marathi',
    'Gujarati',
    'Tamil',
    'Telugu',
    'Kannada',
    'Malayalam',
    'Odia',
    'Marwari',
    'English'
  ];

  educationOptions: string[] = [
    'All Education Levels',
    'Engineering / B.Tech / M.Tech',
    'Management / MBA / PGDM',
    'Medical / Doctor / MBBS',
    'Finance / CA / CS / CFA',
    'Post Graduate / Master’s',
    'Graduate / Bachelor’s'
  ];

  professionOptions: string[] = [
    'All Professions',
    'Software / IT / Tech',
    'Govt / Civil Services / PSU',
    'Banking / Finance / Investment',
    'Healthcare / Doctor / Medical',
    'Business / Entrepreneur',
    'Architecture / Design',
    'Corporate Executive / Management'
  ];

  maritalStatuses: string[] = [
    'All Marital Statuses',
    'Never Married',
    'Divorced',
    'Widowed',
    'Awaiting Divorce'
  ];

  incomeOptions: string[] = [
    'No Bar',
    '₹5 Lakhs & Above',
    '₹10 Lakhs & Above',
    '₹15 Lakhs & Above',
    '₹25 Lakhs & Above',
    '₹50 Lakhs & Above'
  ];

  popularCities: string[] = [
    'All Cities', 'Bengaluru', 'Mumbai', 'Delhi NCR', 'Pune', 'Hyderabad',
    'Chennai', 'Kolkata', 'Ahmedabad', 'Jaipur', 'Chandigarh'
  ];

  constructor(private searchService: SearchService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    // Read query params from URL if navigated from home page quick search
    this.route.queryParams.subscribe(params => {
      if (params['lookingFor']) {
        this.filter.gender = params['lookingFor'];
      }
      if (params['minAge']) {
        this.filter.minAge = +params['minAge'];
      }
      if (params['maxAge']) {
        this.filter.maxAge = +params['maxAge'];
      }
      if (params['religion']) {
        this.filter.religion = params['religion'];
      }
      if (params['motherTongue']) {
        this.filter.motherTongue = params['motherTongue'];
      }
      this.executeSearch();
    });
  }

  executeSearch(): void {
    this.isLoading = true;
    this.searchService.search(this.filter).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.profiles = res.profiles || [];
        this.totalMatches = res.total || this.profiles.length;
        this.applyClientSideSort();
      },
      error: () => {
        this.isLoading = false;
        // Fallback local search simulation if offline
        this.profiles = this.getLocalFilteredProfiles();
        this.totalMatches = this.profiles.length;
        this.applyClientSideSort();
      }
    });
  }

  onSortChange(): void {
    this.applyClientSideSort();
  }

  applyClientSideSort(): void {
    if (this.filter.sortBy === 'age_asc') {
      this.profiles.sort((a, b) => a.age - b.age);
    } else if (this.filter.sortBy === 'age_desc') {
      this.profiles.sort((a, b) => b.age - a.age);
    } else if (this.filter.sortBy === 'income_desc') {
      this.profiles.sort((a, b) => b.annualIncome.localeCompare(a.annualIncome));
    }
  }

  resetFilters(): void {
    this.filter = {
      gender: 'Bride',
      minAge: 21,
      maxAge: 35,
      religion: 'All Religions',
      community: '',
      motherTongue: 'All Languages',
      city: '',
      state: '',
      country: 'India',
      education: 'All Education Levels',
      profession: 'All Professions',
      maritalStatus: 'All Marital Statuses',
      minHeight: "5'0\"",
      maxHeight: "6'0\"",
      minIncome: 'No Bar',
      profileId: '',
      photoOnly: false,
      verifiedOnly: false,
      sortBy: 'relevance'
    };
    this.executeSearch();
    this.showToast('Filters reset to default.');
  }

  sendInterest(profile: SearchResultItem, event: Event): void {
    event.stopPropagation();
    profile.interestSent = !profile.interestSent;
    if (profile.interestSent) {
      this.showToast(`✨ Sent Interest to ${profile.name}! They will be notified.`);
    } else {
      this.showToast(`Interest in ${profile.name} withdrawn.`);
    }
  }

  toggleShortlist(profile: SearchResultItem, event: Event): void {
    event.stopPropagation();
    profile.isShortlisted = !profile.isShortlisted;
    if (profile.isShortlisted) {
      this.showToast(`⭐ Added ${profile.name} to your Shortlist!`);
    } else {
      this.showToast(`Removed from Shortlist.`);
    }
  }

  openDetailModal(profile: SearchResultItem): void {
    this.selectedProfile = profile;
  }

  closeDetailModal(): void {
    this.selectedProfile = null;
  }

  showToast(msg: string): void {
    this.toastMessage = msg;
    setTimeout(() => {
      this.toastMessage = null;
    }, 3500);
  }

  private getLocalFilteredProfiles(): SearchResultItem[] {
    const list: SearchResultItem[] = [
      {
        id: 101,
        profileId: "MS-101",
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
        annualIncome: "₹18 - ₹24 Lakhs",
        location: "Bengaluru, Karnataka",
        city: "Bengaluru",
        state: "Karnataka",
        imageUrl: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Warm, ambitious tech professional who values deep family bonds, Indian culture, and weekend travel."
      },
      {
        id: 102,
        profileId: "MS-102",
        name: "Rohan Deshmukh",
        gender: "Groom",
        age: 28,
        height: "5'11\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Maratha",
        motherTongue: "Marathi",
        education: "MBA - Finance (IIM)",
        profession: "Investment Banker",
        company: "Goldman Sachs",
        annualIncome: "₹25 - ₹35 Lakhs",
        location: "Mumbai, Maharashtra",
        city: "Mumbai",
        state: "Maharashtra",
        imageUrl: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Passionate about finance, fitness, and world cinema. Looking for an understanding and cheerful life companion."
      },
      {
        id: 103,
        profileId: "MS-103",
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
        annualIncome: "₹15 - ₹20 Lakhs",
        location: "Chandigarh / Delhi",
        city: "Chandigarh",
        state: "Punjab",
        imageUrl: "https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Dedicated doctor with a lively personality. Loves music, classical dance, and exploring new culinary experiences."
      },
      {
        id: 104,
        profileId: "MS-104",
        name: "Aditya Patel",
        gender: "Groom",
        age: 29,
        height: "5'10\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Patel",
        motherTongue: "Gujarati",
        education: "MS in AI & Data Science",
        profession: "Product Lead",
        company: "Amazon",
        annualIncome: "₹35 - ₹50 Lakhs",
        location: "Ahmedabad / Hyderabad",
        city: "Ahmedabad",
        state: "Gujarat",
        imageUrl: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Tech entrepreneur at heart. Believes in mutual respect, shared dreams, and lifelong growth together."
      },
      {
        id: 105,
        profileId: "MS-105",
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
        location: "Kolkata / Gurugram",
        city: "Kolkata",
        state: "West Bengal",
        imageUrl: "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Warm-hearted, artistic, and grounded. Enjoys Rabindra Sangeet, reading literature, and weekend cooking."
      },
      {
        id: 106,
        profileId: "MS-106",
        name: "Karthik Ramanathan",
        gender: "Groom",
        age: 30,
        height: "6'0\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Iyer Brahmin",
        motherTongue: "Tamil",
        education: "M.Tech - IIT Madras",
        profession: "Engineering Manager",
        company: "Google",
        annualIncome: "₹45 - ₹60 Lakhs",
        location: "Chennai / Bengaluru",
        city: "Chennai",
        state: "Tamil Nadu",
        imageUrl: "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Calm, thoughtful, and passionate about innovation and Carnatic music. Seeking a supportive life partner."
      },
      {
        id: 107,
        profileId: "MS-107",
        name: "Meera Nair",
        gender: "Bride",
        age: 26,
        height: "5'5\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Nair",
        motherTongue: "Malayalam",
        education: "MBA - Marketing (Symbiosis)",
        profession: "Brand Strategist",
        company: "Unilever",
        annualIncome: "₹16 - ₹22 Lakhs",
        location: "Kochi / Mumbai",
        city: "Kochi",
        state: "Kerala",
        imageUrl: "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: false,
        about: "Creative, energetic, and family-oriented. Passionate about art, travel, and culinary exploration."
      },
      {
        id: 108,
        profileId: "MS-108",
        name: "Varun Kapoor",
        gender: "Groom",
        age: 29,
        height: "5'11\"",
        maritalStatus: "Never Married",
        religion: "Hindu",
        caste: "Khatri Punjabi",
        motherTongue: "Punjabi",
        education: "B.Arch - SPA Delhi",
        profession: "Senior Architect & Partner",
        company: "Kapoor & Associates",
        annualIncome: "₹30 - ₹40 Lakhs",
        location: "Delhi NCR",
        city: "Delhi",
        state: "Delhi",
        imageUrl: "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?auto=format&fit=crop&w=600&q=80",
        verified: true,
        premium: true,
        about: "Architect with an eye for aesthetics and design. Loves hiking, architecture tours, and spending time with family."
      }
    ];

    return list.filter(p => {
      if (this.filter.gender && this.filter.gender !== 'Any') {
        const wantBride = this.filter.gender === 'Bride' || this.filter.gender === 'Female';
        const isBride = p.gender === 'Bride' || p.gender === 'Female';
        if (wantBride !== isBride) return false;
      }
      if (this.filter.minAge && p.age < this.filter.minAge) return false;
      if (this.filter.maxAge && p.age > this.filter.maxAge) return false;
      if (this.filter.religion && this.filter.religion !== 'All Religions') {
        if (p.religion.toLowerCase() !== this.filter.religion.toLowerCase()) return false;
      }
      if (this.filter.motherTongue && this.filter.motherTongue !== 'All Languages') {
        if (p.motherTongue.toLowerCase() !== this.filter.motherTongue.toLowerCase()) return false;
      }
      if (this.filter.city && this.filter.city !== 'All Cities' && this.filter.city.trim() !== '') {
        if (!p.location.toLowerCase().includes(this.filter.city.toLowerCase())) return false;
      }
      if (this.filter.profileId && this.filter.profileId.trim() !== '') {
        if (!p.profileId.toLowerCase().includes(this.filter.profileId.trim().toLowerCase())) return false;
      }
      return true;
    });
  }
}
