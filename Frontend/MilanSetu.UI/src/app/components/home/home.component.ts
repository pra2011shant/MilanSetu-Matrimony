import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslatePipe } from '../../pipes/translate.pipe';

export interface Profile {
  id: string;
  name: string;
  gender: 'Bride' | 'Groom';
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

export interface Story {
  id: string;
  coupleName: string;
  weddingDate: string;
  location: string;
  imageUrl: string;
  quote: string;
  storySnippet: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  // Search Form State
  searchCriteria = {
    lookingFor: 'Bride',
    minAge: 22,
    maxAge: 30,
    religion: 'All Religions',
    motherTongue: 'All Languages'
  };

  // Filter list options
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

  ageOptions: number[] = Array.from({ length: 43 }, (_, i) => 18 + i); // 18 to 60

  activeTab: 'all' | 'brides' | 'grooms' | 'premium' = 'all';

  // Selected Profile for Modal
  selectedProfile: Profile | null = null;
  toastMessage: string | null = null;

  // Profiles Database (Mock Data with rich aesthetics)
  allProfiles: Profile[] = [
    {
      id: 'MS-101',
      name: 'Ananya Sharma',
      gender: 'Bride',
      age: 25,
      height: "5'4\"",
      religion: 'Hindu',
      caste: 'Brahmin',
      motherTongue: 'Hindi',
      education: 'B.Tech - Computer Science',
      profession: 'Senior Software Engineer',
      company: 'Microsoft',
      location: 'Bengaluru, Karnataka',
      imageUrl: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: true,
      about: 'Cheerful, ambitious tech professional who values deep family bonds, Indian culture, and weekend travel.'
    },
    {
      id: 'MS-102',
      name: 'Rohan Deshmukh',
      gender: 'Groom',
      age: 28,
      height: "5'11\"",
      religion: 'Hindu',
      caste: 'Maratha',
      motherTongue: 'Marathi',
      education: 'MBA - Finance (IIM)',
      profession: 'Investment Banker',
      company: 'Goldman Sachs',
      location: 'Mumbai, Maharashtra',
      imageUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: true,
      about: 'Passionate about finance, fitness, and world cinema. Looking for an understanding and cheerful life companion.'
    },
    {
      id: 'MS-103',
      name: 'Simran Kaur Gill',
      gender: 'Bride',
      age: 26,
      height: "5'6\"",
      religion: 'Sikh',
      caste: 'Jat Sikh',
      motherTongue: 'Punjabi',
      education: 'M.D. Pediatrics',
      profession: 'Resident Doctor',
      company: 'Apollo Hospital',
      location: 'Chandigarh / Delhi',
      imageUrl: 'https://images.unsplash.com/photo-1517841905240-472988babdf9?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: false,
      about: 'Dedicated doctor with a lively personality. Loves music, classical dance, and exploring new culinary experiences.'
    },
    {
      id: 'MS-104',
      name: 'Aditya Patel',
      gender: 'Groom',
      age: 29,
      height: "5'10\"",
      religion: 'Hindu',
      caste: 'Patel',
      motherTongue: 'Gujarati',
      education: 'MS in AI & Data Science',
      profession: 'Product Lead',
      company: 'Amazon',
      location: 'Ahmedabad / Hyderabad',
      imageUrl: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: true,
      about: 'Tech entrepreneur at heart. Believes in mutual respect, shared dreams, and lifelong growth together.'
    },
    {
      id: 'MS-105',
      name: 'Pooja Bannerjee',
      gender: 'Bride',
      age: 27,
      height: "5'3\"",
      religion: 'Hindu',
      caste: 'Bengali Brahmin',
      motherTongue: 'Bengali',
      education: 'Chartered Accountant (CA)',
      profession: 'Finance Manager',
      company: 'Deloitte',
      location: 'Kolkata / Gurugram',
      imageUrl: 'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: true,
      about: 'Warm-hearted, artistic, and grounded. Enjoys Rabindra Sangeet, reading literature, and weekend cooking.'
    },
    {
      id: 'MS-106',
      name: 'Karthik Ramanathan',
      gender: 'Groom',
      age: 30,
      height: "6'0\"",
      religion: 'Hindu',
      caste: 'Iyer',
      motherTongue: 'Tamil',
      education: 'M.Tech - IIT Madras',
      profession: 'Engineering Manager',
      company: 'Google',
      location: 'Chennai / Bengaluru',
      imageUrl: 'https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=600&q=80',
      verified: true,
      premium: false,
      about: 'Calm, thoughtful, and passionate about innovation and Carnatic music. Seeking a supportive life partner.'
    }
  ];

  filteredProfiles: Profile[] = [...this.allProfiles];

  // Success Stories
  successStories: Story[] = [
    {
      id: 'S-1',
      coupleName: 'Vikram & Radhika',
      weddingDate: 'December 2025',
      location: 'Jaipur Palace, Rajasthan',
      imageUrl: 'https://images.unsplash.com/photo-1583939003579-730e3918a45a?auto=format&fit=crop&w=600&q=80',
      quote: '"We connected on MilanSetu with just one click, and found a lifetime of unconditional love and laughter!"',
      storySnippet: 'Vikram from Pune and Radhika from Jaipur matched through verified filters. Their shared love for travel and family values led to a beautiful destination wedding.'
    },
    {
      id: 'S-2',
      coupleName: 'Aman & Harpreet',
      weddingDate: 'November 2025',
      location: 'Amritsar, Punjab',
      imageUrl: 'https://images.unsplash.com/photo-1609357605129-26f69add5d6e?auto=format&fit=crop&w=600&q=80',
      quote: '"MilanSetu’s verified profiles gave our families 100% peace of mind and the perfect life companion."',
      storySnippet: 'Both working in healthcare, they found true alignment in aspirations and core Punjabi values within 3 weeks of connecting on the portal.'
    },
    {
      id: 'S-3',
      coupleName: 'Arjun & Sneha',
      weddingDate: 'January 2026',
      location: 'Udaipur, Rajasthan',
      imageUrl: 'https://images.unsplash.com/photo-1511285560929-80b456fea0bc?auto=format&fit=crop&w=600&q=80',
      quote: '"Found my soulmate who understands my career goals and cherishes cultural traditions equally."',
      storySnippet: 'From our first chat on MilanSetu to meeting each other’s families, everything felt naturally right. Forever grateful!'
    }
  ];

  ngOnInit() {
    this.applyTabFilter('all');
  }

  // Handle Hero Quick Search
  onSearch() {
    this.filteredProfiles = this.allProfiles.filter(p => {
      const matchGender = this.searchCriteria.lookingFor === 'Bride' ? p.gender === 'Bride' : p.gender === 'Groom';
      const matchAge = p.age >= this.searchCriteria.minAge && p.age <= this.searchCriteria.maxAge;
      const matchReligion = this.searchCriteria.religion === 'All Religions' || p.religion.toLowerCase() === this.searchCriteria.religion.toLowerCase();
      const matchLanguage = this.searchCriteria.motherTongue === 'All Languages' || p.motherTongue.toLowerCase() === this.searchCriteria.motherTongue.toLowerCase();

      return matchGender && matchAge && matchReligion && matchLanguage;
    });

    // Smooth scroll to results
    const element = document.getElementById('featured');
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }

    this.showToast(`Found ${this.filteredProfiles.length} compatible profile(s) matching your criteria!`);
  }

  applyTabFilter(tab: 'all' | 'brides' | 'grooms' | 'premium') {
    this.activeTab = tab;
    if (tab === 'all') {
      this.filteredProfiles = [...this.allProfiles];
    } else if (tab === 'brides') {
      this.filteredProfiles = this.allProfiles.filter(p => p.gender === 'Bride');
    } else if (tab === 'grooms') {
      this.filteredProfiles = this.allProfiles.filter(p => p.gender === 'Groom');
    } else if (tab === 'premium') {
      this.filteredProfiles = this.allProfiles.filter(p => p.premium);
    }
  }

  sendInterest(profile: Profile, event: Event) {
    event.stopPropagation();
    profile.interestSent = !profile.interestSent;
    if (profile.interestSent) {
      this.showToast(`✨ Expressed Interest in ${profile.name}! Notification sent.`);
    } else {
      this.showToast(`Interest in ${profile.name} withdrawn.`);
    }
  }

  openProfileModal(profile: Profile) {
    this.selectedProfile = profile;
  }

  closeProfileModal() {
    this.selectedProfile = null;
  }

  showToast(msg: string) {
    this.toastMessage = msg;
    setTimeout(() => {
      this.toastMessage = null;
    }, 3800);
  }
}
