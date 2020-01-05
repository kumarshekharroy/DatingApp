import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlerifyServices } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() hideRegistrationFormEmmitter = new EventEmitter();
  model: any = {};

  constructor(
    private authService: AuthService,
    private alertify: AlerifyServices
  ) {}

  ngOnInit() {}

  hideRegistrationForm() {
    this.hideRegistrationFormEmmitter.emit(false);
  }
  registerUser() {
    this.authService.register(this.model).subscribe(
      () => {
        this.alertify.success('Registration successful.');
      },
      error => this.alertify.error(error)
    );
  }
}
