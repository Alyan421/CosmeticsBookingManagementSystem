import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../Authorization/auth.service';
import { CategoryService } from '../Categories/category.service';
import { BrandService } from '../Brands/brand.service';
import { ImageService } from '../Images/image.service';
import { ProductService } from '../Product/product.service';
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
    categories: 0,
    brands: 0,
    images: 0,
    product: 0
  };
  isLoading: boolean = true;

  constructor(
    private authService: AuthService,
    private categoryService: CategoryService,
    private brandService: BrandService,
    private imageService: ImageService,
    private productService: ProductService
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
      categories: this.categoryService.getAllCategories().pipe(catchError(error => {
        console.error('Error fetching categories:', error);
        return of([]);
      })),
      brands: this.brandService.getAllBrands().pipe(catchError(error => {
        console.error('Error fetching brands:', error);
        return of([]);
      })),
      images: this.imageService.getAllImages().pipe(catchError(error => {
        console.error('Error fetching images:', error);
        return of([]);
      })),
      products: this.productService.getAllProduct().pipe(catchError(error => {
        console.error('Error fetching product:', error);
        return of([]);
      }))
    }).subscribe({
      next: (results) => {
        this.cardCount = {
          categories: results.categories.length,
          brands: results.brands.length,
          images: results.images.length,
          product: results.products.length
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
