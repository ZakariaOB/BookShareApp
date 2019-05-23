import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor() { }

  ngOnInit() {
  }

  login() {
    console.log('Login here');
    console.log(this.model.Name);
    console.log(this.model.Password);
  }
}
