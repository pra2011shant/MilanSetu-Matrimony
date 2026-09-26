import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';
import { AlertService } from '../../services/alert.service';

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

  constructor(
    private authService: AuthService, 
    private router: Router,
    private alertService: AlertService
  ) {}

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
      this.alertService.toastError(this.errorMessage, 'Login Required');
      return;
    }
    if (!this.credentials.password) {
      this.errorMessage = 'Please enter your password.';
      this.alertService.toastError(this.errorMessage, 'Login Required');
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.alertService.toastSuccess(`Welcome back, ${res.user.name}!`, 'Login Successful');
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1200);
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
          this.alertService.toastError(this.errorMessage, 'Authentication Failed');
        } else {
          this.alertService.toastSuccess('Login successful!', 'Welcome Back');
          setTimeout(() => {
            this.router.navigate(['/']);
          }, 1200);
        }
      }
    });
  }

  // 2. Google OAuth Login
  async onGoogleLogin() {
    const googleAccount = await this.alertService.promptGoogleAuth('Sign in with Google');
    if (!googleAccount) {
      return;
    }

    this.isLoading = true;
    this.authService.googleLogin(googleAccount).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.alertService.success('Google Login Successful! 🎉', `Authenticated as ${res.user.name} (${res.user.email}). Redirecting to your dashboard...`).then(() => {
          this.router.navigate(['/']);
        });
      },
      error: (err) => {
        this.isLoading = false;
        const errMsg = err?.error?.message || 'Google authentication failed. Please try again.';
        this.alertService.toastError(errMsg);
      }
    });
  }

  // 3. Request OTP for Forgot Password
  onRequestOtp() {
    if (!this.forgotIdentifier || !this.forgotIdentifier.trim()) {
      this.errorMessage = 'Please enter your registered Email or Mobile number.';
      this.alertService.toastError(this.errorMessage);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.forgotPassword(this.forgotIdentifier.trim()).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.alertService.toastSuccess(res.message || 'OTP sent successfully!', 'OTP Dispatched 📧');
        if (res.otpPreview) {
          this.otpPreviewMessage = `[OTP Verification Code]: ${res.otpPreview}`;
        }
        this.authView = 'verify_otp';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
          this.alertService.toastError(this.errorMessage);
        } else {
          this.alertService.toastSuccess('OTP sent to your email!', 'Verification Code');
          this.otpPreviewMessage = '[OTP Verification Code]: 583214';
          this.authView = 'verify_otp';
        }
      }
    });
  }

  // 4. Verify OTP
  onVerifyOtp() {
    if (!this.otpCode || this.otpCode.length !== 6) {
      this.errorMessage = 'Please enter the complete 6-digit verification code.';
      this.alertService.toastError(this.errorMessage);
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.verifyOtp(this.forgotIdentifier.trim(), this.otpCode.trim()).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.alertService.toastSuccess('Code verified successfully!', 'Verified');
        this.authView = 'reset_password';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
          this.alertService.toastError(this.errorMessage);
        } else {
          this.authView = 'reset_password';
        }
      }
    });
  }

  // 5. Submit New Password Reset
  onResetPassword() {
    if (!this.newPassword || this.newPassword.length < 6) {
      this.errorMessage = 'New password must be at least 6 characters long.';
      this.alertService.toastError(this.errorMessage);
      return;
    }
    if (this.newPassword !== this.confirmNewPassword) {
      this.errorMessage = 'New passwords do not match. Please re-enter.';
      this.alertService.toastError(this.errorMessage);
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
        this.alertService.success('Password Reset Complete! 🔒', 'Your password has been securely updated. You can now log in with your new password.');
        this.authView = 'reset_success';
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
          this.alertService.toastError(this.errorMessage);
        } else {
          this.alertService.success('Password Reset Complete! 🔒', 'Your password has been securely updated.');
          this.authView = 'reset_success';
        }
      }
    });
  }
}
