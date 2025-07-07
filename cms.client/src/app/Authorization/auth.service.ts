import { Injectable } from '@angular/core';
import { jwtDecode} from 'jwt-decode';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private tokenKey = 'authToken';

  // Save the token in localStorage
  saveToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  // Retrieve the token
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Remove the token on logout
  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decoded: any = jwtDecode(token);
    const currentTime = Math.floor(Date.now() / 1000);
    return decoded.exp < currentTime;
  }

  getCurrentUser(): any {
    const token = this.getToken();
    if (!token) return null;
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  }

  getCurrentUserRole(): string | null {
    const user = this.getCurrentUser();

    // Check multiple potential role property names
    if (user) {
      // The "role" claim (simplified version)
      if (user.role) return user.role;

      // The standard JWT role claim
      if (user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        return user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      // Find any property that might contain role info
      const roleProp = Object.keys(user).find(key =>
        key.toLowerCase().includes('role') ||
        (typeof user[key] === 'string' &&
          ['admin', 'user'].includes(user[key].toLowerCase()))
      );

      if (roleProp) return user[roleProp];
    }

    return null;
  }

}
