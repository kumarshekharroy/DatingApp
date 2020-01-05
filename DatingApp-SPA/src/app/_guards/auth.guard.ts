import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { AlerifyServices } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private alertify: AlerifyServices,
    private router: Router
  ) {}
  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    }
    this.alertify.error('Fall back..!! You shall not pass!!!');
    this.router.navigate(['/home']);
    return false;
  }
}
