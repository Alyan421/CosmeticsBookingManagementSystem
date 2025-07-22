import { Component, OnInit } from '@angular/core';
import { ProductService, Product, ProductCreate, ProductUpdate } from './product.service';
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
  paginatedProducts: Product[] = [];
  brands: any[] = [];
  categories: any[] = [];
  productForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  isEditing = false;
  isCreating = false;
  searchTerm = '';
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;
  editingProductId: number | null = null;

  constructor(
    private productService: ProductService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private notification: NotificationService,
    private route: ActivatedRoute
  ) {
    this.productForm = this.fb.group({
      id: [null],
      brandId: [null, Validators.required],
      categoryId: [null, Validators.required],
      productName: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      availableProduct: [0, [Validators.required, Validators.min(0)]],
      price: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadData();

    this.route.queryParams.subscribe(params => {
      if (params['categoryId']) {
        const categoryId = +params['categoryId'];
        this.searchTerm = '';

        this.productService.getProductsByCategory(categoryId).subscribe({
          next: (data) => {
            this.products = data;
            this.applyFiltersAndPagination();
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error('Failed to load product data for category');
            console.error('Error loading product data:', error);
            this.isLoading = false;
          }
        });
      } else {
        this.loadData();
      }
    });
  }

  loadData(): void {
    this.isLoading = true;

    forkJoin({
      brands: this.brandService.getAllBrands(),
      categories: this.categoryService.getAllCategories(),
      products: this.productService.getAllProduct()
    }).subscribe({
      next: (data) => {
        this.brands = data.brands;
        this.categories = data.categories;
        this.products = data.products;
        this.applyFiltersAndPagination();
        this.isLoading = false;
      },
      error: (error) => {
        this.notification.error('Failed to load data');
        console.error('Error loading data:', error);
        this.isLoading = false;
      }
    });
  }

  applyFiltersAndPagination(): void {
    if (!this.searchTerm) {
      this.filteredProducts = [...this.products];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredProducts = this.products.filter(product =>
        product.brandName.toLowerCase().includes(term) ||
        product.categoryName.toLowerCase().includes(term) ||
        product.productName.toLowerCase().includes(term) ||
        (product.description && product.description.toLowerCase().includes(term))
      );
    }

    this.totalItems = this.filteredProducts.length;
    this.paginate();
  }

  paginate(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.paginatedProducts = this.filteredProducts.slice(startIndex, endIndex);
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.getTotalPages()) {
      this.currentPage = page;
      this.paginate();
    }
  }

  onSearch(): void {
    this.currentPage = 1;
    this.applyFiltersAndPagination();
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

  startCreating(): void {
    this.isCreating = true;
    this.isEditing = false;
    this.editingProductId = null;
    this.resetFormForCreation();
    this.enableAllFormControls();

    // Scroll to form after a short delay
    setTimeout(() => {
      const formElement = document.getElementById('productForm');
      if (formElement) {
        formElement.scrollIntoView({ behavior: 'smooth' });
      }
    }, 100);
  }

  startEditing(product: Product): void {
    this.isEditing = true;
    this.isCreating = false;
    this.editingProductId = product.id;

    this.productForm.patchValue({
      id: product.id,
      brandId: product.brandId,
      categoryId: product.categoryId,
      productName: product.productName,
      description: product.description || '',
      availableProduct: product.availableProduct,
      price: product.price
    });

    this.enableAllFormControls();

    setTimeout(() => {
      document.getElementById('productForm')?.scrollIntoView({ behavior: 'smooth' });
    }, 100);
  }

  submitForm(): void {
    if (this.productForm.invalid) {
      this.markFormGroupTouched(this.productForm);
      this.notification.error('Please fill in all required fields correctly');
      return;
    }

    this.isSubmitting = true;

    if (this.isCreating) {
      this.createProduct();
    } else if (this.isEditing) {
      this.updateProduct();
    } else {
      this.isSubmitting = false;
    }
  }

  createProduct(): void {
    const formValue = this.productForm.value;

    const productData: ProductCreate = {
      brandId: parseInt(formValue.brandId),
      categoryId: parseInt(formValue.categoryId),
      productName: formValue.productName.trim(),
      description: formValue.description?.trim() || undefined,
      availableProduct: parseInt(formValue.availableProduct),
      price: parseFloat(formValue.price)
    };

    this.productService.createProduct(productData).subscribe({
      next: (response) => {
        this.products.push(response);
        this.notification.success('Product created successfully');
        this.resetForm();
        this.applyFiltersAndPagination();
        this.isSubmitting = false;
      },
      error: (error) => {
        let errorMessage = 'Failed to create product';

        if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        } else if (typeof error.error === 'string') {
          errorMessage = error.error;
        }

        this.notification.error(errorMessage);
        this.isSubmitting = false;
      }
    });
  }

  updateProduct(): void {
    const formValue = this.productForm.value;

    const productData: ProductUpdate = {
      id: formValue.id,
      brandId: parseInt(formValue.brandId),
      categoryId: parseInt(formValue.categoryId),
      productName: formValue.productName.trim(),
      description: formValue.description?.trim() || undefined,
      availableProduct: parseInt(formValue.availableProduct),
      price: parseFloat(formValue.price)
    };

    this.productService.updateProduct(productData).subscribe({
      next: (response) => {
        const index = this.products.findIndex(p => p.id === productData.id);
        if (index !== -1) {
          this.products[index] = response;
        }

        this.notification.success('Product updated successfully');
        this.resetForm();
        this.applyFiltersAndPagination();
        this.isSubmitting = false;
      },
      error: (error) => {
        let errorMessage = 'Failed to update product';

        if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.message) {
          errorMessage = error.message;
        }

        this.notification.error(errorMessage);
        this.isSubmitting = false;
      }
    });
  }

  deleteProduct(product: Product): void {
    if (confirm(`Are you sure you want to delete "${product.productName}"?`)) {
      this.productService.deleteProduct(product.id).subscribe({
        next: () => {
          this.products = this.products.filter(p => p.id !== product.id);
          this.applyFiltersAndPagination();
          this.notification.success('Product deleted successfully');
        },
        error: (error) => {
          let errorMessage = 'Failed to delete product';
          if (error.error?.message) {
            errorMessage = error.error.message;
          }
          this.notification.error(errorMessage);
          console.error('Error deleting product:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.isEditing = false;
    this.isCreating = false;
    this.editingProductId = null;

    this.productForm.reset({
      id: null,
      brandId: null,
      categoryId: null,
      productName: '',
      description: '',
      availableProduct: 0,
      price: 0
    });

    this.enableAllFormControls();
  }

  resetFormForCreation(): void {
    this.productForm.reset({
      id: null,
      brandId: null,
      categoryId: null,
      productName: '',
      description: '',
      availableProduct: 0,
      price: 0
    });

    this.enableAllFormControls();
  }

  enableAllFormControls(): void {
    Object.keys(this.productForm.controls).forEach(key => {
      this.productForm.get(key)?.enable();
    });
  }

  markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  getBrandName(brandId: number): string {
    const brand = this.brands.find(b => b.id === brandId);
    return brand ? brand.name : 'Unknown';
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.categoryName : 'Unknown';
  }

  exportToCSV(): void {
    const headers = ['ID', 'Product Name', 'Brand', 'Category', 'Description', 'Available Product', 'Price', 'Image URL'];
    const csvData = this.products.map(product => [
      product.id,
      product.productName,
      product.brandName,
      product.categoryName,
      product.description || '',
      product.availableProduct,
      product.price,
      product.imageUrl || ''
    ]);

    let csvContent = headers.join(',') + '\n';
    csvData.forEach(row => {
      const escapedRow = row.map(cell => {
        const cellStr = String(cell);
        return cellStr.includes(',') || cellStr.includes('"') ? `"${cellStr.replace(/"/g, '""')}"` : cellStr;
      });
      csvContent += escapedRow.join(',') + '\n';
    });

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', 'products.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.productForm.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['maxlength']) return `${fieldName} is too long`;
      if (field.errors['min']) return `${fieldName} must be positive`;
    }
    return '';
  }

  getQuantityClass(quantity: number): string {
    if (quantity === 0) {
      return 'quantity-out text-danger';
    } else if (quantity <= 5) {
      return 'quantity-low text-warning';
    } else if (quantity <= 20) {
      return 'quantity-medium text-warning';
    } else {
      return 'quantity-high text-success';
    }
  }
}
