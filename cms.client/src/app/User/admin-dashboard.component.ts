import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../Authorization/auth.service';
import { ColorService } from '../Colors/color.service';
import { ClothService } from '../Cloths/cloth.service';
import { ImageService } from '../Images/image.service';
import { StockService } from '../Stock/stock.service';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-component.dashboard.css'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ]
})
export class AdminDashboardComponent implements OnInit {
  adminName: string = 'Admin'; // Provide a default value
  lastLogin: string = '';
  cardCount = {
    colors: 0,
    cloths: 0,
    images: 0,
    stock: 0
  };
  isLoading: boolean = true;

  constructor(
    private authService: AuthService,
    private colorService: ColorService,
    private clothService: ClothService,
    private imageService: ImageService,
    private stockService: StockService
  ) { }

  ngOnInit(): void {
    // Get current user info
    try {
      const user = this.authService.getCurrentUser();
      if (user) {
        this.adminName = user.name || user.email || 'Admin';
        this.lastLogin = new Date().toLocaleString(); // Or get from user profile if available
      }
    } catch (error) {
      console.error('Error getting current user:', error);
      // Keep the default admin name
    }

    // Fetch real stats from services
    this.fetchDashboardStats();
  }

  private fetchDashboardStats(): void {
    this.isLoading = true;

    // Use forkJoin to make parallel requests
    forkJoin({
      colors: this.colorService.getAllColors().pipe(catchError(error => {
        console.error('Error fetching colors:', error);
        return of([]);
      })),
      cloths: this.clothService.getAllCloths().pipe(catchError(error => {
        console.error('Error fetching cloths:', error);
        return of([]);
      })),
      images: this.imageService.getAllImages().pipe(catchError(error => {
        console.error('Error fetching images:', error);
        return of([]);
      })),
      stocks: this.stockService.getAllStock().pipe(catchError(error => {
        console.error('Error fetching stock:', error);
        return of([]);
      }))
    }).subscribe({
      next: (results) => {
        this.cardCount = {
          colors: results.colors.length,
          cloths: results.cloths.length,
          images: results.images.length,
          stock: results.stocks.length
        };
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching dashboard stats:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
