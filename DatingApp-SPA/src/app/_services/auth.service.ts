import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { from } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  private jwtHelper = new JwtHelperService();
  public decodedToken: any;
  constructor(private http: HttpClient) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        if (response) {
          localStorage.setItem('token', response.token);
          this.decodedToken = this.jwtHelper.decodeToken(response.token);
        }
      })
    );
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }
  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
