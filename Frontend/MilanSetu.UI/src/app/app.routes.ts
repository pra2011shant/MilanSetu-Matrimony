import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { MyProfileComponent } from './components/my-profile/my-profile.component';
import { PartnerPreferenceComponent } from './components/partner-preference/partner-preference.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'my-profile', component: MyProfileComponent },
  { path: 'partner-preference', component: PartnerPreferenceComponent },
  { path: '**', redirectTo: '' }
];





