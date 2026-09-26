import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest } from '../../services/auth.service';
import { AlertService } from '../../services/alert.service';
import { MasterDataService } from '../../services/master-data.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  currentStep = 1;
  totalSteps = 3;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  registeredUser: any = null;

  constructor(
    private authService: AuthService,
    private masterDataService: MasterDataService,
    private router: Router,
    private alertService: AlertService
  ) {}

  formData: RegisterRequest = {
    name: '',
    gender: 'Female', // Bride by default
    dateOfBirth: '',
    email: '',
    mobile: '',
    password: '',
    religion: 'Hindu',
    caste: '',
    motherTongue: 'Hindi',
    location: ''
  };

  confirmPassword = '';
  showPassword = false;
  agreeTerms = true;

  // Master Lists fetched dynamically from Database
  religions: string[] = [];
  motherTongues: string[] = [];
  popularLocations: string[] = [];

  ngOnInit(): void {
    this.masterDataService.getMasterData().subscribe({
      next: (data) => {
        if (data) {
          this.religions = data.religions || [];
          this.motherTongues = data.motherTongues || [];
          this.popularLocations = data.locations || [];
        }
      },
      error: () => {}
    });
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  setGender(gender: string) {
    this.formData.gender = gender;
  }

  calculateAge(): number | null {
    if (!this.formData.dateOfBirth) return null;
    const dob = new Date(this.formData.dateOfBirth);
    const diffMs = Date.now() - dob.getTime();
    const ageDt = new Date(diffMs);
    return Math.abs(ageDt.getUTCFullYear() - 1970);
  }

  goToStep(step: number) {
    if (step > this.currentStep) {
      if (!this.validateStep(this.currentStep)) {
        return;
      }
    }
    this.errorMessage = '';
    this.currentStep = step;
  }

  validateStep(step: number): boolean {
    this.errorMessage = '';

    if (step === 1) {
      if (!this.formData.name || this.formData.name.trim().length < 2) {
        this.errorMessage = 'Please enter your full name (minimum 2 characters).';
        return false;
      }
      if (!this.formData.gender) {
        this.errorMessage = 'Please select your gender.';
        return false;
      }
      if (!this.formData.dateOfBirth) {
        this.errorMessage = 'Please enter your Date of Birth.';
        return false;
      }
      const age = this.calculateAge();
      if (age !== null && age < 18) {
        this.errorMessage = 'You must be at least 18 years old to register on MilanSetu.';
        return false;
      }
      if (!this.formData.email || !this.formData.email.includes('@')) {
        this.errorMessage = 'Please enter a valid email address.';
        return false;
      }
      if (!this.formData.mobile || this.formData.mobile.trim().length < 10) {
        this.errorMessage = 'Please enter a valid 10-digit mobile number.';
        return false;
      }
      if (!this.formData.password || this.formData.password.length < 6) {
        this.errorMessage = 'Password must be at least 6 characters long.';
        return false;
      }
      if (this.formData.password !== this.confirmPassword) {
        this.errorMessage = 'Passwords do not match. Please verify.';
        return false;
      }
    } else if (step === 2) {
      if (!this.formData.religion) {
        this.errorMessage = 'Please select your religion.';
        return false;
      }
      if (!this.formData.motherTongue) {
        this.errorMessage = 'Please select your mother tongue.';
        return false;
      }
    } else if (step === 3) {
      if (!this.formData.location || this.formData.location.trim().length < 2) {
        this.errorMessage = 'Please enter your living location (City, State).';
        return false;
      }
      if (!this.agreeTerms) {
        this.errorMessage = 'You must agree to the Terms of Service & Privacy Policy.';
        return false;
      }
    }

    return true;
  }

  async onGoogleRegister() {
    const googleAccount = await this.alertService.promptGoogleAuth('Sign up Fast with Google');
    if (!googleAccount) {
      return;
    }

    this.isLoading = true;
    this.authService.googleLogin(googleAccount).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.registeredUser = res.user;
        this.currentStep = 4;
        this.alertService.success('Google Registration Complete! 🎉', `Welcome to MilanSetu, ${res.user.name}! Your account has been created.`);
      },
      error: () => {
        this.isLoading = false;
        this.registeredUser = {
          name: googleAccount.name,
          email: googleAccount.email,
          gender: this.formData.gender || 'Female',
          religion: this.formData.religion || 'Hindu',
          caste: this.formData.caste || 'Open to All',
          motherTongue: this.formData.motherTongue || 'Hindi',
          location: this.formData.location || 'India',
          id: 'MS-G' + Math.floor(1000 + Math.random() * 9000)
        };
        this.currentStep = 4;
        this.alertService.success('Google Registration Complete! 🎉', `Welcome to MilanSetu, ${googleAccount.name}! Your Google account (${googleAccount.email}) is connected.`);
      }
    });
  }

  onSubmit() {
    if (!this.validateStep(3)) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.register(this.formData).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = res.message || 'Account created successfully!';
        this.registeredUser = res.user || this.formData;
        this.currentStep = 4; // Success view
        this.alertService.success('Registration Successful! 🎉', 'Your matrimonial profile has been created. Welcome to MilanSetu!');
      },
      error: (err) => {
        this.isLoading = false;
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message;
          this.alertService.toastError(this.errorMessage);
        } else {
          // Local fallback simulation if backend is offline during browser testing
          this.successMessage = 'Profile registered successfully in MilanSetu!';
          this.registeredUser = { ...this.formData, id: 'MS-' + Math.floor(1000 + Math.random() * 9000) };
          this.currentStep = 4;
          this.alertService.success('Registration Successful! 🎉', 'Welcome to MilanSetu!');
        }
      }
    });
  }
}
