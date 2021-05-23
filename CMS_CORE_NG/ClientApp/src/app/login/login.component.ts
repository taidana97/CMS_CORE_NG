import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  imageUrl: string = '/assets/images/bg-register.jpeg';

  constructor() { }

  ngOnInit(): void {
    this.setbackgroundImage();
  }

  setbackgroundImage() {
    $('body').css({
      'background-image': 'url(' + this.imageUrl + ')',
      'background-size': 'cover'
    });
  }

}
