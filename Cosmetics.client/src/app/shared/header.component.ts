import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../Authorization/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  standalone: true,
  imports: [RouterModule, CommonModule],
})
export class HeaderComponent implements OnInit {
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  isMobileMenuOpen: boolean = false;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.checkLoginStatus();
  }

  checkLoginStatus(): void {
    // Check if user is logged in
    this.isLoggedIn = !!this.authService.getToken();
    this.isAdmin = this.authService.getCurrentUserRole() === 'Admin';
  }

  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
    this.isAdmin = false;
    this.router.navigate(['/login']);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
    // Add a class to the body to prevent scrolling when menu is open
    if (this.isMobileMenuOpen) {
      document.body.classList.add('menu-open');
    } else {
      document.body.classList.remove('menu-open');
    }
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
    document.body.classList.remove('menu-open');
  }
}
