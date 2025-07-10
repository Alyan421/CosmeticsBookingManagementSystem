import { Component, OnInit } from '@angular/core';
import { ProductService, Product, ProductUpdate } from './product.service';
import { BrandService } from '../Brands/brand.service';
import { CategoryService } from '../Categories/category.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NotificationService } from '../core/notification.service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-product-management',
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ]
})
export class ProductManagementComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  brands: any[] = [];
  categories: any[] = [];
  productForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  isEditing = false;
  searchTerm = '';
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;

  constructor(
    private productService: ProductService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private notification: NotificationService,
    private route: ActivatedRoute
  ) {
    this.productForm = this.fb.group({
      brandId: ['', Validators.required],
      categoryId: ['', Validators.required],
      availableProduct: [0, [Validators.required, Validators.min(0)]],
      price: [0, [Validators.required, Validators.min(0)]] // Added price field with validation
    });
  }

  ngOnInit(): void {
    this.loadData();

    // Check for query parameters
    this.route.queryParams.subscribe(params => {
      if (params['categoryId']) {
        const categoryId = +params['categoryId'];
        this.searchTerm = ''; // Clear any existing search

        // Filter by the specified category ID
        this.productService.getProductByCategory(categoryId).subscribe({
          next: (data) => {
            this.products = data;
            this.filteredProducts = [...this.products];
            this.totalItems = this.filteredProducts.length;
            this.paginate();
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error('Failed to load product data for category');
            console.error('Error loading product data:', error);
            this.isLoading = false;
          }
        });
      } else {
        // Load all product data as normal
        this.loadData();
      }
    });
  }

  loadData(): void {
    this.isLoading = true;

    // Load brands, categories, and products in parallel
    forkJoin({
      brands: this.brandService.getAllBrands(),
      categories: this.categoryService.getAllCategories(),
      products: this.productService.getAllProduct()
    }).subscribe({
      next: (data) => {
        this.brands = data.brands;
        this.categories = data.categories;
        this.products = data.products;
        this.filterProducts();
        this.isLoading = false;
      },
      error: (error) => {
        this.notification.error('Failed to load data');
        console.error('Error loading data:', error);
        this.isLoading = false;
      }
    });
  }

  filterProducts(): void {
    if (!this.searchTerm) {
      this.filteredProducts = [...this.products];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredProducts = this.products.filter(product =>
        product.brandName.toLowerCase().includes(term) ||
        product.categoryName.toLowerCase().includes(term)
      );
    }

    this.totalItems = this.filteredProducts.length;
    this.paginate();
  }

  paginate(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    this.filteredProducts = [...this.products].slice(startIndex, startIndex + this.itemsPerPage);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.paginate();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.filterProducts();
  }

  getMinValue(a: number, b: number): number {
    return Math.min(a, b);
  }

  getTotalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  getPagesArray(): number[] {
    return Array(this.getTotalPages()).fill(0).map((x, i) => i);
  }

  submitForm(): void {
    if (this.productForm.invalid) {
      this.notification.error('Please fill in all required fields correctly');
      return;
    }

    this.isSubmitting = true;

    // Use getRawValue() instead of value to include disabled controls
    const productData: ProductUpdate = this.productForm.getRawValue();

    this.productService.updateProduct(productData).subscribe({
      next: (response) => {
        // Only handle update case now
        const index = this.products.findIndex(s =>
          s.brandId === productData.brandId && s.categoryId === productData.categoryId);

        if (index !== -1) {
          this.products[index] = response;
          this.notification.success('Product updated successfully');
        }

        this.resetForm();
        this.filterProducts();
        this.isSubmitting = false;
      },
      error: (error) => {
        this.notification.error(error.error || 'Failed to update product');
        console.error('Error updating product:', error);
        this.isSubmitting = false;
      }
    });
  }

  editProduct(product: Product): void {
    this.isEditing = true;
    this.productForm.patchValue({
      brandId: product.brandId,
      categoryId: product.categoryId,
      availableProduct: product.availableProduct,
      price: product.price // Include price when editing
    });

    // Disable brand and category selection during edit
    this.productForm.get('brandId')?.disable();
    this.productForm.get('categoryId')?.disable();

    // Scroll to form
    document.getElementById('productForm')?.scrollIntoView({ behavior: 'smooth' });
  }

  deleteProduct(product: Product): void {
    if (confirm(`Are you sure you want to delete product for ${product.brandName} in ${product.categoryName}?`)) {
      this.productService.deleteProduct(product.brandId, product.categoryId).subscribe({
        next: () => {
          this.products = this.products.filter(s =>
            !(s.brandId === product.brandId && s.categoryId === product.categoryId));
          this.filterProducts();
          this.notification.success('Product deleted successfully');
        },
        error: (error) => {
          this.notification.error(error.error || 'Failed to delete product');
          console.error('Error deleting product:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.isEditing = false; // Reset the isEditing flag
    this.productForm.reset({
      brandId: '',
      categoryId: '',
      availableProduct: 0,
      price: 0 // Include default price when resetting
    });

    // Enable brand and category selection
    this.productForm.get('brandId')?.enable();
    this.productForm.get('categoryId')?.enable();
  }

  getBrandName(brandId: number): string {
    const brand = this.brands.find(c => c.id === brandId);
    return brand ? brand.name : 'Unknown';
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.categoryName : 'Unknown';
  }

  exportToCSV(): void {
    const headers = ['Brand ID', 'Brand Name', 'Category ID', 'Category Name', 'Available Product', 'Price'];
    const csvData = this.products.map(product => [
      product.brandId,
      product.brandName,
      product.categoryId,
      product.categoryName,
      product.availableProduct,
      product.price
    ]);

    // Create CSV content
    let csvContent = headers.join(',') + '\n';
    csvData.forEach(row => {
      csvContent += row.join(',') + '\n';
    });

    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', 'product_inventory.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
