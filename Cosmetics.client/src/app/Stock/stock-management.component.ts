import { Component, OnInit } from '@angular/core';
import { StockService, Stock, StockUpdate } from './stock.service';
import { BrandService } from '../Brands/brand.service';
import { CategoryService } from '../Categories/category.service';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NotificationService } from '../core/notification.service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-stock-management',
  templateUrl: './stock-management.component.html',
  styleUrls: ['./stock-management.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ]
})
export class StockManagementComponent implements OnInit {
  stocks: Stock[] = [];
  filteredStocks: Stock[] = [];
  brands: any[] = [];
  categories: any[] = [];
  stockForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  isEditing = false; // Add this variable declaration
  searchTerm = '';
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;

  constructor(
    private stockService: StockService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private notification: NotificationService,
    private route: ActivatedRoute
  ) {
    this.stockForm = this.fb.group({
      brandId: ['', Validators.required],
      categoryId: ['', Validators.required],
      availableStock: [0, [Validators.required, Validators.min(0)]]
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
        this.stockService.getStockByCategory(categoryId).subscribe({
          next: (data) => {
            this.stocks = data;
            this.filteredStocks = [...this.stocks];
            this.totalItems = this.filteredStocks.length;
            this.paginate();
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error('Failed to load stock data for category');
            console.error('Error loading stock data:', error);
            this.isLoading = false;
          }
        });
      } else {
        // Load all stock data as normal
        this.loadData();
      }
    });
  }

  loadData(): void {
    this.isLoading = true;

    // Load brands, categories, and stocks in parallel
    forkJoin({
      brands: this.brandService.getAllBrands(),
      categories: this.categoryService.getAllCategories(),
      stocks: this.stockService.getAllStock()
    }).subscribe({
      next: (data) => {
        this.brands = data.brands;
        this.categories = data.categories;
        this.stocks = data.stocks;
        this.filterStocks();
        this.isLoading = false;
      },
      error: (error) => {
        this.notification.error('Failed to load data');
        console.error('Error loading data:', error);
        this.isLoading = false;
      }
    });
  }

  filterStocks(): void {
    if (!this.searchTerm) {
      this.filteredStocks = [...this.stocks];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredStocks = this.stocks.filter(stock =>
        stock.brandName.toLowerCase().includes(term) ||
        stock.categoryName.toLowerCase().includes(term)
      );
    }

    this.totalItems = this.filteredStocks.length;
    this.paginate();
  }

  paginate(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    this.filteredStocks = [...this.stocks].slice(startIndex, startIndex + this.itemsPerPage);
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.paginate();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.filterStocks();
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
    if (this.stockForm.invalid) {
      this.notification.error('Please fill in all required fields correctly');
      return;
    }

    this.isSubmitting = true;

    // Use getRawValue() instead of value to include disabled controls
    const stockData: StockUpdate = this.stockForm.getRawValue();

    this.stockService.updateStock(stockData).subscribe({
      next: (response) => {
        // Only handle update case now
        const index = this.stocks.findIndex(s =>
          s.brandId === stockData.brandId && s.categoryId === stockData.categoryId);

        if (index !== -1) {
          this.stocks[index] = response;
          this.notification.success('Stock updated successfully');
        }

        this.resetForm();
        this.filterStocks();
        this.isSubmitting = false;
      },
      error: (error) => {
        this.notification.error(error.error || 'Failed to update stock');
        console.error('Error updating stock:', error);
        this.isSubmitting = false;
      }
    });
  }

  editStock(stock: Stock): void {
    this.isEditing = true;
    this.stockForm.patchValue({
      brandId: stock.brandId,
      categoryId: stock.categoryId,
      availableStock: stock.availableStock
    });

    // Disable brand and category selection during edit
    this.stockForm.get('brandId')?.disable();
    this.stockForm.get('categoryId')?.disable();

    // Scroll to form
    document.getElementById('stockForm')?.scrollIntoView({ behavior: 'smooth' });
  }

  deleteStock(stock: Stock): void {
    if (confirm(`Are you sure you want to delete stock for ${stock.brandName} in ${stock.categoryName}?`)) {
      this.stockService.deleteStock(stock.brandId, stock.categoryId).subscribe({
        next: () => {
          this.stocks = this.stocks.filter(s =>
            !(s.brandId === stock.brandId && s.categoryId === stock.categoryId));
          this.filterStocks();
          this.notification.success('Stock deleted successfully');
        },
        error: (error) => {
          this.notification.error(error.error || 'Failed to delete stock');
          console.error('Error deleting stock:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.isEditing = false; // Reset the isEditing flag
    this.stockForm.reset({
      brandId: '',
      categoryId: '',
      availableStock: 0
    });

    // Enable brand and category selection
    this.stockForm.get('brandId')?.enable();
    this.stockForm.get('categoryId')?.enable();
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
    const headers = ['Brand ID', 'Brand Name', 'Category ID', 'Category Name', 'Available Stock'];
    const csvData = this.stocks.map(stock => [
      stock.brandId,
      stock.brandName,
      stock.categoryId,
      stock.categoryName,
      stock.availableStock
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
    link.setAttribute('download', 'stock_inventory.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
