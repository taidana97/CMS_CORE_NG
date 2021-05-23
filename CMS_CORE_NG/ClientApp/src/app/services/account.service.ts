import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { map } from 'rxjs/operators';
import { Register } from '../interfaces/register';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private baseUrlRegister: string = '/api/v1/account/register';

  registerModel: Register;

  constructor(private http: HttpClient, private cookieService: CookieService) {}

  register(
    username: string,
    firstname: string,
    lastname: string,
    password: string,
    email: string,
    country: string,
    phone: string,
    gender: string,
    dob: string,
    terms: boolean
  ) {
    this.registerModel = {
      username: username,
      firstname: firstname,
      lastname: lastname,
      password: password,
      email: email,
      country: country,
      phone: phone,
      gender: gender,
      dob: dob,
      terms: terms,
    };

    return this.http
      .post<any>(this.baseUrlRegister, this.registerModel, {
        headers: {
          'Content-Type': 'application/json',
          'No-Auth': 'True',
          'X-XSRF-TOKEN': this.cookieService.get('XSRF-TOKEN'),
        },
      })
      .pipe(
        map(
          (result) => {
            return result;
          },
          (error: any) => {
            return error;
          }
        )
      );
  }
}
