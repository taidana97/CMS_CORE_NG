import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { UserSettingsComponent } from './user-settings/user-settings.component';
import { UserActivityComponent } from './user-activity/user-activity.component';


@NgModule({
  declarations: [UserProfileComponent, UserSettingsComponent, UserActivityComponent],
  imports: [
    CommonModule,
    UserRoutingModule
  ]
})
export class UserModule { }
