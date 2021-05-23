import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

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
