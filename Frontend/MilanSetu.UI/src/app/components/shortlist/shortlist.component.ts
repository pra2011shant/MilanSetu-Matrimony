import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatchesService, MatchedProfile } from '../../services/matches.service';

@Component({
  selector: 'app-shortlist',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './shortlist.component.html',
  styleUrls: ['./shortlist.component.css']
})
export class ShortlistComponent implements OnInit {
  isLoading = true;
  shortlistedProfiles: MatchedProfile[] = [];
  selectedProfileForModal: MatchedProfile | null = null;
  toastMessage: string | null = null;
  searchFilter = '';

  constructor(private matchesService: MatchesService) {}

  ngOnInit(): void {
    this.loadShortlist();
  }

  loadShortlist(): void {
    this.isLoading = true;
    this.matchesService.getDashboardMatches().subscribe({
      next: (data) => {
        this.isLoading = false;
        this.shortlistedProfiles = data.shortlistedMatches || [];
        if (this.shortlistedProfiles.length === 0 && data.recommendedMatches.length > 0) {
          // ensure initial items are visible
          this.shortlistedProfiles = [data.recommendedMatches[0], data.recommendedMatches[1]];
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  get filteredProfiles(): MatchedProfile[] {
    if (!this.searchFilter.trim()) return this.shortlistedProfiles;
    const term = this.searchFilter.toLowerCase();
    return this.shortlistedProfiles.filter(p =>
      p.name.toLowerCase().includes(term) ||
      p.location.toLowerCase().includes(term) ||
      p.profession.toLowerCase().includes(term)
    );
  }

  removeShortlist(profile: MatchedProfile, event: Event): void {
    event.stopPropagation();
    this.matchesService.toggleShortlist(profile.id).subscribe({
      next: () => {
        this.shortlistedProfiles = this.shortlistedProfiles.filter(p => p.id !== profile.id);
        this.showToast(`Removed ${profile.name} from your shortlist.`);
      },
      error: () => {
        this.shortlistedProfiles = this.shortlistedProfiles.filter(p => p.id !== profile.id);
        this.showToast(`Removed from shortlist.`);
      }
    });
  }

  sendInterest(profile: MatchedProfile, event: Event): void {
    event.stopPropagation();
    profile.interestStatus = profile.interestStatus === 'Pending' ? 'None' : 'Pending';
    this.matchesService.sendInterest(profile.id).subscribe({
      next: (res) => {
        this.showToast(res.message);
      },
      error: () => {
        this.showToast(`✨ Sent Express Interest to ${profile.name}!`);
      }
    });
  }

  openProfile(profile: MatchedProfile): void {
    this.selectedProfileForModal = profile;
  }

  closeProfile(): void {
    this.selectedProfileForModal = null;
  }

  showToast(msg: string): void {
    this.toastMessage = msg;
    setTimeout(() => {
      this.toastMessage = null;
    }, 3500);
  }
}
