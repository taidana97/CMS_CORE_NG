import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { SendCodeComponent } from './send-code/send-code.component';
import { TermsComponent } from './terms/terms.component';
import { ValidateCodeComponent } from './validate-code/validate-code.component';
import { UserComponent } from './user/user.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
<<<<<<< HEAD
<<<<<<< HEAD
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
=======
>>>>>>> 3fce1492d26f4df0131e093a19ae9572bb1420be
=======
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50

@NgModule({
  declarations: [
    AppComponent,
    AboutUsComponent,
    ContactUsComponent,
    ForgotPasswordComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
    SendCodeComponent,
    TermsComponent,
    ValidateCodeComponent,
    UserComponent,
<<<<<<< HEAD
<<<<<<< HEAD
    NavMenuComponent,
=======
    NavMenuComponent
>>>>>>> 3fce1492d26f4df0131e093a19ae9572bb1420be
=======
    NavMenuComponent,
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
