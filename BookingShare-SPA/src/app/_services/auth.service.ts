import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baserUrl = 'http://localhost:5000/api/auth/';

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baserUrl + 'login', model)
      .pipe(map(this.mapLoginResponse));
  }

  mapLoginResponse(response: any) {
    const user = response;
    if (user) {
      localStorage.setItem('token', user.token);
    }
  }

  register(model: any) {
    return this.http.post(this.baserUrl + 'register', model)
  }
}
