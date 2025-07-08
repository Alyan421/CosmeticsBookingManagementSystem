import { Component, OnInit } from '@angular/core';
import { UserService } from './user.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
})
export class UserProfileComponent implements OnInit {
  user: any = {};

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    // Fetch user profile from backend
    this.userService.getAllUsers().subscribe((data) => {
      this.user = data[0]; // Example: Load the first user
    });
  }
}
