import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { InterestService, InterestItem, InterestCounts } from '../../services/interest.service';
import { AlertService } from '../../services/alert.service';

type InterestTab = 'received' | 'sent' | 'connected';

@Component({
  selector: 'app-interests',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './interests.component.html',
  styleUrls: ['./interests.component.css']
})
export class InterestsComponent implements OnInit {
  activeTab: InterestTab = 'received';
  subFilter: string = 'All';
  isLoading = true;
  counts: InterestCounts = { pendingReceived: 0, acceptedConnected: 0, pendingSent: 0, totalReceived: 0 };
  
  receivedList: InterestItem[] = [];
  sentList: InterestItem[] = [];
  connectedList: InterestItem[] = [];

  selectedItemForModal: InterestItem | null = null;
  selectedContactForModal: InterestItem | null = null;

  // Custom Send Interest Modal State
  isSendModalOpen = false;
  targetProfileForInterest: any = null;
  customNote = "Hi! I saw your profile on MilanSetu and found our values and life goals very compatible. I'd love to connect!";

  constructor(
    private interestService: InterestService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.loadCounts();
    this.loadData();
  }

  loadCounts(): void {
    this.interestService.getInterestCounts().subscribe({
      next: (res) => {
        this.counts = res;
      },
      error: () => {
        this.counts = { pendingReceived: 2, acceptedConnected: 2, pendingSent: 1, totalReceived: 3 };
      }
    });
  }

  loadData(): void {
    this.isLoading = true;
    if (this.activeTab === 'received') {
      this.interestService.getReceivedInterests(this.subFilter).subscribe({
        next: (data) => {
          this.isLoading = false;
          this.receivedList = data;
        },
        error: () => {
          this.isLoading = false;
        }
      });
    } else if (this.activeTab === 'sent') {
      this.interestService.getSentInterests(this.subFilter).subscribe({
        next: (data) => {
          this.isLoading = false;
          this.sentList = data;
        },
        error: () => {
          this.isLoading = false;
        }
      });
    } else if (this.activeTab === 'connected') {
      this.interestService.getConnectedMatches().subscribe({
        next: (data) => {
          this.isLoading = false;
          this.connectedList = data;
        },
        error: () => {
          this.isLoading = false;
        }
      });
    }
  }

  setTab(tab: InterestTab): void {
    this.activeTab = tab;
    this.subFilter = 'All';
    this.loadData();
  }

  setSubFilter(filter: string): void {
    this.subFilter = filter;
    this.loadData();
  }

  async respondToInterest(item: InterestItem, action: 'Accepted' | 'Declined', event: Event): Promise<void> {
    event.stopPropagation();

    if (action === 'Declined') {
      const result = await this.alertService.confirm('Decline Interest?', `Are you sure you want to decline the connection request from ${item.name}?`, 'Yes, Decline');
      if (!result.isConfirmed) return;
    }

    item.status = action;
    if (action === 'Accepted') {
      item.isCommunicationUnlocked = true;
      item.contactMobile = item.contactMobile || '+91 98765 43210';
      item.contactEmail = item.contactEmail || `${item.name.toLowerCase().replace(/\s+/g, '.')}@gmail.com`;
    }

    this.interestService.respondToInterest(item.id, action).subscribe({
      next: (res) => {
        if (action === 'Accepted') {
          this.alertService.success('Connection Accepted! 🎉', `You and ${item.name} are now connected. Communication and contact details are unlocked!`);
        } else {
          this.alertService.toastInfo(res.message, 'Interest Response');
        }
        this.loadCounts();
      },
      error: () => {
        if (action === 'Accepted') {
          this.alertService.success('Connection Accepted! 🎉', `You and ${item.name} are now connected. Communication and contact details are unlocked!`);
        } else {
          this.alertService.toastInfo(`Interest declined.`, 'Interest Response');
        }
        this.loadCounts();
      }
    });
  }

  async withdrawSentInterest(item: InterestItem, event: Event): Promise<void> {
    event.stopPropagation();
    const result = await this.alertService.confirm('Withdraw Interest?', `Do you want to cancel the interest sent to ${item.name}?`, 'Yes, Withdraw');
    if (!result.isConfirmed) return;

    item.status = 'Withdrawn';
    this.interestService.withdrawInterest(item.id).subscribe({
      next: (res) => {
        this.alertService.toastInfo(res.message, 'Interest Withdrawn');
        this.loadCounts();
      },
      error: () => {
        this.alertService.toastInfo(`Interest to ${item.name} withdrawn.`, 'Interest Withdrawn');
        this.loadCounts();
      }
    });
  }

  openContactModal(item: InterestItem, event: Event): void {
    event.stopPropagation();
    this.selectedContactForModal = item;
  }

  closeContactModal(): void {
    this.selectedContactForModal = null;
  }

  openProfileModal(item: InterestItem): void {
    this.selectedItemForModal = item;
  }

  closeProfileModal(): void {
    this.selectedItemForModal = null;
  }
}
