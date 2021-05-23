import { Injectable } from '@angular/core';
import { AbstractControl, ValidatorFn } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class ValidatorService {
  constructor() {}

  MustMatch(firstControl: AbstractControl): ValidatorFn {
    return (
      secondControl: AbstractControl
    ): { [key: string]: boolean } | null => {
      if (!firstControl && !secondControl) {
        return null;
      }

      if (secondControl.hasError && !firstControl.hasError) {
        return null;
      }

      if (firstControl.value !== secondControl.value) {
        return { mustMatch: true };
      } else {
        return null;
      }
    };
  }
}
