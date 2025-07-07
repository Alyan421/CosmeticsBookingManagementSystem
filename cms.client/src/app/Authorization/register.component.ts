import { Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../User/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule], // Add RouterModule
})
export class RegisterComponent {
  registerForm: FormGroup;
  registrationSuccess: boolean = false;
  errorMessage: string = '';

  // Add Router to the constructor
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  // Method to handle form submission
  register(): void {
    if (this.registerForm.valid) {
      // Call the UserService's registerUser method
      this.userService.registerUser(this.registerForm.value).subscribe({
        next: () => {
          this.registrationSuccess = true;
          this.errorMessage = '';
          this.registerForm.reset();

          // Redirect to public gallery after a short delay
          setTimeout(() => {
            this.router.navigate(['/public-gallery']);
          }, 2000); // 2 second delay
        },
        error: (err) => {
          this.registrationSuccess = false;
          this.errorMessage =
            err.error?.message || 'Registration failed. Please try again.';
        },
      });
    } else {
      this.errorMessage = 'Please fill out the form correctly.';
    }
  }
}
