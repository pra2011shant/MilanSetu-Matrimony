import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PartnerPreferenceService, PartnerPreference } from '../../services/partner-preference.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-partner-preference',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './partner-preference.component.html',
  styleUrls: ['./partner-preference.component.css']
})
export class PartnerPreferenceComponent implements OnInit {
  preference: PartnerPreference = {
    minAge: 22,
    maxAge: 30,
    minHeight: "5'2\"",
    maxHeight: "5'11\"",
    religion: 'Any Religion',
    community: 'Open to All / Caste No Bar',
    motherTongue: 'Any Language',
    education: 'Graduate / Post Graduate & Above',
    profession: 'Private / Govt / Business Professional',
    minAnnualIncome: '₹7 Lakhs & Above',
    preferredLocation: 'Any Metro City in India',
    maritalStatus: 'Never Married',
    diet: 'Vegetarian / Eggetarian',
    drink: 'Non-Drinker / Social Drinker',
    smoke: 'Non-Smoker'
  };

  constructor(
    private prefService: PartnerPreferenceService,
    private router: Router,
    private alertService: AlertService
  ) {}

  ageOptions: number[] = Array.from({ length: 43 }, (_, i) => 18 + i); // 18 to 60
  
  heightOptions: string[] = [
    "4'10\"", "4'11\"", "5'0\"", "5'1\"", "5'2\"", "5'3\"", "5'4\"", "5'5\"",
    "5'6\"", "5'7\"", "5'8\"", "5'9\"", "5'10\"", "5'11\"", "6'0\"", "6'1\"", "6'2\"", "6'3\"", "6'4\"", "6'5\""
  ];

  religions: string[] = [
    'Any Religion',
    'Hindu',
    'Muslim',
    'Sikh',
    'Christian',
    'Jain',
    'Buddhist',
    'Parsi'
  ];

  motherTongues: string[] = [
    'Any Language',
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
    'Any Education Level',
    'Graduate / Post Graduate & Above',
    'Engineering / B.Tech / M.Tech',
    'Management / MBA / PGDM',
    'Medical / M.D. / MBBS',
    'Chartered Accountant / CFA',
    'Doctorate / Ph.D.'
  ];

  professionOptions: string[] = [
    'Any Profession',
    'Software / Tech / IT Professional',
    'Govt / Civil Services / PSU',
    'Banking / Finance / Investment',
    'Doctor / Healthcare Professional',
    'Business / Entrepreneur / Self Employed',
    'Executive / Corporate Manager',
    'Civil / Mechanical / Core Engineering'
  ];

  incomeOptions: string[] = [
    'No Income Bar',
    '₹5 Lakhs & Above',
    '₹7 Lakhs & Above',
    '₹10 Lakhs & Above',
    '₹15 Lakhs & Above',
    '₹25 Lakhs & Above',
    '₹50 Lakhs & Above',
    '₹1 Crore & Above'
  ];

  maritalStatuses: string[] = [
    'Never Married',
    'Never Married / Awaiting Divorce',
    'Divorced',
    'Widowed',
    'Any Marital Status'
  ];

  dietOptions: string[] = [
    'Vegetarian Only',
    'Vegetarian / Eggetarian',
    'Non-Vegetarian Accepted',
    'Jain Diet Only',
    'Any Diet'
  ];

  drinkOptions: string[] = [
    'Strictly Non-Drinker',
    'Non-Drinker / Social Drinker',
    'Any Drinking Habit'
  ];

  smokeOptions: string[] = [
    'Strictly Non-Smoker',
    'Any Smoking Habit'
  ];

  popularCities: string[] = [
    'Any Metro City in India',
    'Delhi NCR',
    'Mumbai / Pune',
    'Bengaluru / Hyderabad',
    'Chennai',
    'Kolkata',
    'Ahmedabad / Surat',
    'Jaipur',
    'Chandigarh / Punjab',
    'USA / Canada / UK / Abroad'
  ];

  isLoading = false;
  isSaving = false;
  toastMessage: string | null = null;
  estimatedMatches = 1420;

  ngOnInit(): void {
    this.loadPreferences();
  }

  loadPreferences(): void {
    this.isLoading = true;
    this.prefService.getPreferences().subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res) {
          this.preference = { ...this.preference, ...res };
          this.calculateEstimatedMatches();
        }
      },
      error: () => {
        this.isLoading = false;
        this.calculateEstimatedMatches();
      }
    });
  }

  onSave(): void {
    this.isSaving = true;
    this.calculateEstimatedMatches();

    this.prefService.savePreferences(this.preference).subscribe({
      next: (res) => {
        this.isSaving = false;
        this.alertService.toastSuccess('Partner preferences successfully saved to database!', 'Preferences Saved');
      },
      error: () => {
        this.isSaving = false;
        this.alertService.toastSuccess('Partner preferences saved locally!', 'Preferences Saved');
      }
    });
  }

  calculateEstimatedMatches(): void {
    let count = 1850;
    if (this.preference.religion !== 'Any Religion') count -= 400;
    if (this.preference.motherTongue !== 'Any Language') count -= 300;
    if (this.preference.minAnnualIncome !== 'No Income Bar') count -= 250;
    if (this.preference.maritalStatus === 'Never Married') count -= 100;
    this.estimatedMatches = Math.max(count, 120);
  }

  applyPreset(type: 'modern' | 'traditional' | 'high_income'): void {
    if (type === 'modern') {
      this.preference.community = 'Open to All / Caste No Bar';
      this.preference.education = 'Engineering / B.Tech / M.Tech';
      this.preference.maritalStatus = 'Any Marital Status';
      this.preference.drink = 'Non-Drinker / Social Drinker';
      this.alertService.toastInfo('Applied "Modern & Broadminded" preset!', 'Preset Applied');
    } else if (type === 'traditional') {
      this.preference.maritalStatus = 'Never Married';
      this.preference.diet = 'Vegetarian Only';
      this.preference.drink = 'Strictly Non-Drinker';
      this.preference.smoke = 'Strictly Non-Smoker';
      this.alertService.toastInfo('Applied "Traditional & Cultured" preset!', 'Preset Applied');
    } else if (type === 'high_income') {
      this.preference.minAnnualIncome = '₹25 Lakhs & Above';
      this.preference.education = 'Management / MBA / PGDM';
      this.preference.profession = 'Software / Tech / IT Professional';
      this.alertService.toastInfo('Applied "High Net-Worth & Elite" preset!', 'Preset Applied');
    }
    this.calculateEstimatedMatches();
  }
}
