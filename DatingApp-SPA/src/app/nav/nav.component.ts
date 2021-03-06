import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlerifyServices } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(
    public authService: AuthService,
    private alertify: AlerifyServices,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentProfilePic.subscribe(
      photoUrl => (this.photoUrl = photoUrl)
    );
  }
  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Login Successful');
      },
      error => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }
  loggedIn() {
    return this.authService.isLoggedIn();
  }
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }
}
