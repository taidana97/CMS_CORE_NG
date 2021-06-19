import { Component, OnInit } from '@angular/core';
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import Swal from 'sweetalert2';
import { Country } from '../interfaces/country';
import { AccountService } from '../services/account.service';
import { ValidatorService } from '../services/common/validator.service';
import { CountryService } from '../services/country.service';
<<<<<<< HEAD
=======
>>>>>>> 3fce1492d26f4df0131e093a19ae9572bb1420be
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  imageUrl: string = '/assets/images/bg-register.jpeg';

  country$: Observable<Country[]>;
  countries: Country[];
  registerForm: FormGroup;
  errorList: string[];
  modelMessage: string;
  modelTitle: string;
  isRegistrationInProcess: boolean = false;
  username: FormControl;
  firstname: FormControl;
  lastname: FormControl;
  email: FormControl;
  confirmEmail: FormControl;
  password: FormControl;
  cpassword: FormControl;
  country: FormControl;
  phone: FormControl;
  gender: FormControl;
  dob: FormControl;
  terms: FormControl;

  constructor(
    private countryService: CountryService,
    private formBuilder: FormBuilder,
    private validatorService: ValidatorService,
    public toastService: ToastrService,
    private accountService: AccountService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initComponent();

    this.setbackgroundImage();
    this.loadCountries();
  }

  initComponent() {
    this.username = new FormControl('', [
      Validators.required,
      Validators.maxLength(10),
      Validators.minLength(5),
    ]);
    this.firstname = new FormControl('', [
      Validators.required,
      Validators.maxLength(10),
      Validators.minLength(5),
    ]);
    this.lastname = new FormControl('', [
      Validators.required,
      Validators.maxLength(10),
      Validators.minLength(5),
    ]);
    this.email = new FormControl('', [Validators.required, Validators.email]);
    this.confirmEmail = new FormControl('', [
      Validators.required,
      this.validatorService.MustMatch(this.email),
    ]);
    this.password = new FormControl('', [
      Validators.required,
      Validators.maxLength(10),
      Validators.minLength(5),
    ]);
    this.cpassword = new FormControl('', [
      Validators.required,
      this.validatorService.MustMatch(this.password),
    ]);
    this.country = new FormControl('', [Validators.required]);
    this.phone = new FormControl('', [
      Validators.required,
      Validators.pattern(
        '^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$'
      ),
    ]);
    this.gender = new FormControl('', [Validators.required]);
    this.dob = new FormControl('', [Validators.required]);
    this.terms = new FormControl('', [Validators.required]);

    this.registerForm = this.formBuilder.group({
      username: this.username,
      firstname: this.firstname,
      lastname: this.lastname,
      email: this.email,
      confirmEmail: this.confirmEmail,
      password: this.password,
      cpassword: this.cpassword,
      country: this.country,
      phone: this.phone,
      gender: this.gender,
      dob: this.dob,
      terms: this.terms,
    });

    $(() => {
      $('#datepicker').datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: '1920:2099',
        onSelect: (dateText: any) => {
          this.dob.setValue(dateText);
        },
      });
    });
  }

  showModalError() {
    this.modelTitle = 'Registration Error';
    this.modelMessage = 'Your registration was unsuccessful';
    $('#errorModel').modal('show');
<<<<<<< HEAD
=======
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  imageUrl: string = '/assets/images/bg-register.jpeg';

  constructor() { }

  ngOnInit(): void {
    this.setbackgroundImage();
>>>>>>> 3fce1492d26f4df0131e093a19ae9572bb1420be
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
  }

  setbackgroundImage() {
    $('body').css({
      'background-image': 'url(' + this.imageUrl + ')',
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
      'background-size': 'cover',
    });
  }

  loadCountries() {
    this.country$ = this.countryService.getCountries();
    this.country$.subscribe((countryList) => {
      this.countries = countryList;
    });
  }

  onSubmit(): void {
    this.isRegistrationInProcess = true;
    let userDetails = this.registerForm.value;
    // this.accountService
    //   .register(
    //     userDetails.username,
    //     userDetails.firstname,
    //     userDetails.lastname,
    //     userDetails.password,
    //     userDetails.email,
    //     userDetails.country,
    //     userDetails.phone,
    //     userDetails.gender,
    //     userDetails.dob,
    //     userDetails.terms
    //   )
    //   .subscribe((result) => {
    //     if (result.message == 'Registration Successful') {
    //       let timerInterval: NodeJS.Timeout;
    //       Swal.fire({
    //         title: 'Registration was Successful!',
    //         html: 'Please check your email for verification email.<br>Redirecting to login page <b></b>.',
    //         timer: 10000,
    //         allowOutsideClick: false,
    //         onBeforeOpen: () => {
    //           Swal.showLoading();

    //           timerInterval = setInterval(() => {
    //             var textContentSwal =
    //               Swal.getContent()?.querySelector('b')?.textContent;
    //             var getTimerLeft: number = Swal.getTimerLeft()!;

    //             textContentSwal = String((getTimerLeft / 1000).toFixed(0));
    //           }, 100);
    //         },
    //         onClose: () => {
    //           clearInterval(timerInterval);
    //         },
    //       }).then((r) => {
    //         Swal.stopTimer();
    //         this.router.navigate(['/login']);
    //       });

    //       this.isRegistrationInProcess = false;
    //     }
    //   });
  }
<<<<<<< HEAD
=======
      'background-size': 'cover'
    });
  }

>>>>>>> 3fce1492d26f4df0131e093a19ae9572bb1420be
=======
>>>>>>> 450a87848e75769b635f5c1f59a126e3ebe82c50
}
