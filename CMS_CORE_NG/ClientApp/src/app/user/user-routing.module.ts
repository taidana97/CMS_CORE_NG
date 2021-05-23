import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserActivityComponent } from './user-activity/user-activity.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';

const routes: Routes = [
  { path: '', component: UserProfileComponent },
  { path: 'myaccount', component: UserProfileComponent },
  { path: 'settings', component: UserSettingsComponent },
  { path: 'activity', component: UserActivityComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
