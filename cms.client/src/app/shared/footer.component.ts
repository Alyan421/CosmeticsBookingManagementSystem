import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../Authorization/auth.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css'],
  standalone: true,
  imports: [RouterModule, CommonModule],
})
export class FooterComponent implements OnInit {
  currentYear: number = new Date().getFullYear();
  isAdmin: boolean = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.checkAdminStatus();
  }

  checkAdminStatus(): void {
    const role = this.authService.getCurrentUserRole();
    this.isAdmin = role === 'Admin';
  }
}
