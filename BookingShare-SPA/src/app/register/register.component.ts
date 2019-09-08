import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { InnerSubscriber } from 'rxjs/internal/InnerSubscriber';
import { User } from '../_models/User';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  user: User;

  constructor(
    private authService: AuthService,
    private alertifyService: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-blue',
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(8),
          ],
        ],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.passwordMatchValidator }
    );
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => {
          this.alertifyService.message('Registration successful');
        },
        error => {
          this.alertifyService.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  initFormGroup() {
    this.registerForm = new FormGroup(
      {
        username: new FormControl('', Validators.required),
        password: new FormControl('', [
          Validators.maxLength(8),
          Validators.required,
          Validators.minLength(4),
        ]),
        confirmPassword: new FormControl(),
      },
      this.passwordMatchValidator
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }
}
