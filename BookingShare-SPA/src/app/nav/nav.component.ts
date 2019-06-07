import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  public authService: AuthService;
  
  constructor(_authService: AuthService, private alertifyJs: AlertifyService, private router: Router) {
    this.authService = _authService;
  }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertifyJs.success('Login success');
      this.router.navigate([ '/members' ]);
    },
      error => {
        this.alertifyJs.error(error);
      });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertifyJs.message('Logged out');
    this.router.navigate([ '/home' ]);
  }
}
