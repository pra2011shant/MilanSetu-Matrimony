import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NotificationService, AppNotification } from '../../services/notification.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isMenuOpen = false;
  isScrolled = false;
  isNotificationsOpen = false;
  notifications: AppNotification[] = [];

  constructor(
    public authService: AuthService,
    public notifService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.notifService.getNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
      },
      error: () => {}
    });
  }

  get unreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  toggleNotifications(): void {
    this.isNotificationsOpen = !this.isNotificationsOpen;
  }

  markAllRead(): void {
    this.notifService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.forEach(n => { n.isRead = true; });
      }
    });
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  logout() {
    this.authService.logout();
  }
}


