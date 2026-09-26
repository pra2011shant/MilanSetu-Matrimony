
import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { MyProfileComponent } from './components/my-profile/my-profile.component';
import { PartnerPreferenceComponent } from './components/partner-preference/partner-preference.component';
import { SearchComponent } from './components/search/search.component';
import { MatchesComponent } from './components/matches/matches.component';
import { InterestsComponent } from './components/interests/interests.component';
import { ShortlistComponent } from './components/shortlist/shortlist.component';
import { ChatComponent } from './components/chat/chat.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'matches', component: MatchesComponent },
  { path: 'dashboard', component: MatchesComponent },
  { path: 'interests', component: InterestsComponent },
  { path: 'inbox', component: InterestsComponent },
  { path: 'chat', component: ChatComponent },
  { path: 'shortlist', component: ShortlistComponent },
  { path: 'search', component: SearchComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'my-profile', component: MyProfileComponent },
  { path: 'profile', redirectTo: 'my-profile', pathMatch: 'full' },
  { path: 'partner-preference', component: PartnerPreferenceComponent },
  { path: 'preference', redirectTo: 'partner-preference', pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];





