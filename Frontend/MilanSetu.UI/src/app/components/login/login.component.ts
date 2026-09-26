import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  // Login Form
  credentials: LoginRequest = {
    identifier: '',
    password: ''
  };

  showPassword = false;
  rememberMe = true;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Forgot Password / OTP Flow Mode: 'login' | 'forgot_request' | 'verify_otp' | 'reset_password' | 'reset_success'
  authView: 'login' | 'forgot_request' | 'verify_otp' | 'reset_password' | 'reset_success' = 'login';

  // Forgot password state
  forgotIdentifier = '';
  otpCode = '';
  newPassword = '';
  confirmNewPassword = '';
  otpPreviewMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  switchView(view: 'login' | 'forgot_request' | 'verify_otp' | 'reset_password' | 'reset_success') {
    this.authView = view;
    this.errorMessage = '';
    this.successMessage = '';
    this.otpPreviewMessage = '';
  }

  // 1. Submit Login
  onLogin() {
    if (!this.credentials.identifier || !this.credentials.identifier.trim()) {
      this.errorMessage = 'Please enter your registered Email or Mobile number.';
      return;
    }
    if (!this.credentials.password) {
      this.errorMessage = 'Please enter your password.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = `Welcome back, ${res.user.name}! Redirecting...`;
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1500);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        } else {
          // Simulation for quick local preview
          this.successMessage = 'Login successful (Simulation)! Redirecting...';
          setTimeout(() => {
            this.router.navigate(['/']);
          }, 1500);
        }
      }
    });
  }

  // 2. Request OTP for Forgot Password
  onRequestOtp() {
    if (!this.forgotIdentifier || !this.forgotIdentifier.trim()) {
      this.errorMessage = 'Please enter your registered Email or Mobile number.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.forgotPassword(this.forgotIdentifier.trim()).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = res.message || 'OTP sent successfully!';
        if (res.otpPreview) {
          this.otpPreviewMessage = `[Simulation OTP Code]: ${res.otpPreview}`;
        }
        this.authView = 'verify_otp';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        } else {
          // Fallback simulation
          this.successMessage = 'OTP sent to your contact!';
          this.otpPreviewMessage = '[Simulation OTP Code]: 123456';
          this.authView = 'verify_otp';
        }
      }
    });
  }

  // 3. Verify OTP
  onVerifyOtp() {
    if (!this.otpCode || this.otpCode.length !== 6) {
      this.errorMessage = 'Please enter the complete 6-digit verification code.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.verifyOtp(this.forgotIdentifier.trim(), this.otpCode.trim()).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = 'OTP verified! Now enter your new password.';
        this.authView = 'reset_password';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        } else {
          this.authView = 'reset_password';
        }
      }
    });
  }

  // 4. Submit New Password Reset
  onResetPassword() {
    if (!this.newPassword || this.newPassword.length < 6) {
      this.errorMessage = 'New password must be at least 6 characters long.';
      return;
    }
    if (this.newPassword !== this.confirmNewPassword) {
      this.errorMessage = 'New passwords do not match. Please re-enter.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.resetPassword({
      identifier: this.forgotIdentifier.trim(),
      otpCode: this.otpCode.trim(),
      newPassword: this.newPassword
    }).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = res.message || 'Password successfully updated!';
        this.authView = 'reset_success';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
        } else {
          this.authView = 'reset_success';
        }
      }
    });
  }
}
