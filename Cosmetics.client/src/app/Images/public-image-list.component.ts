import { Component, OnInit } from '@angular/core';
import { ImageService } from '../Images/image.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../Categories/category.service';
import { BrandService } from '../Brands/brand.service';

@Component({
  selector: 'app-public-image-list',
  templateUrl: './public-image-list.component.html',
  styleUrls: ['./public-image-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class PublicImageListComponent implements OnInit {
  images: any[] = [];
  filteredImages: any[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 30;
  categories: any[] = [];
  brands: any[] = []; // Add brands array
  selectedCategoryName: string = '';
  selectedBrandName: string = ''; // Add selected brand name
  viewMode: string = 'grid';
  selectedImage: any = null;
  categoriesMap: Map<number, string> = new Map();
  brandsMap: Map<number, string> = new Map(); // Add brands map
  currency: string = 'PKR'; // Set currency to PKR

  // Price filter properties
  minPrice: number = 0;
  maxPrice: number = 1000;
  priceRange: { min: number, max: number } = { min: 0, max: 1000 };
  isFilterActive: boolean = false;

  constructor(
    private imageService: ImageService,
    private categoryService: CategoryService,
    private brandService: BrandService // Add brand service
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadCategories();
    this.loadBrands(); // Load brands on init
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe((data) => {
      this.categories = data.filter(
        (category, index, self) =>
          index === self.findIndex((c) => c.categoryName === category.categoryName)
      );

      data.forEach(category => {
        this.categoriesMap.set(category.id, category.categoryName);
      });
    });
  }

  // Add method to load brands
  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((data) => {
      this.brands = data;

      // Create a map of brandId to brandName for quick lookups
      data.forEach(brand => {
        this.brandsMap.set(brand.id, brand.name);
      });
    });
  }

  filterByCategoryName(categoryName: string): void {
    this.selectedCategoryName = categoryName;
    this.imageService.filterByCategoryName(categoryName).subscribe((data) => {
      this.images = data;
      this.applyFilters();
      this.currentPage = 1;
    });
  }

  // Add method to filter by brand name
  filterByBrandName(brandName: string): void {
    this.selectedBrandName = brandName;
    this.imageService.filterByBrandName(brandName).subscribe((data) => {
      this.images = data;
      this.applyFilters();
      this.currentPage = 1;
    });
  }

  loadImages(): void {
    this.selectedCategoryName = '';
    this.selectedBrandName = '';
    this.imageService.getAllImages().subscribe((data) => {
      this.images = data;

      // Set price range based on actual data
      if (data.length > 0) {
        const prices = data.map(img => img.price).filter(price => price !== undefined && price > 0);
        if (prices.length > 0) {
          this.minPrice = Math.floor(Math.min(...prices));
          this.maxPrice = Math.ceil(Math.max(...prices));
          this.priceRange = { min: this.minPrice, max: this.maxPrice };
        }
      }

      this.applyFilters();
      this.currentPage = 1;
    });
  }

  applyFilters(): void {
    this.filteredImages = this.images.filter(image => {
      // Apply price filter
      const priceInRange = image.price >= this.priceRange.min &&
        image.price <= this.priceRange.max;

      // Check if filter is active
      this.isFilterActive = this.selectedCategoryName !== '' ||
        this.selectedBrandName !== '' || // Add brand name to filter check
        this.priceRange.min > this.minPrice ||
        this.priceRange.max < this.maxPrice;

      return priceInRange;
    });
    this.currentPage = 1;
  }

  onCategoryFilterChange(): void {
    if (this.selectedCategoryName) {
      this.filterByCategoryName(this.selectedCategoryName);
    } else {
      // When "All Categories" is selected (empty value)
      this.loadImages();
    }
  }

  // Add method for brand filter change
  onBrandFilterChange(): void {
    if (this.selectedBrandName) {
      this.filterByBrandName(this.selectedBrandName);
    } else {
      // When "All Brands" is selected (empty value)
      this.loadImages();
    }
  }

  onPriceFilterChange(): void {
    this.applyFilters();
  }

  // Update clear filters to include brand filter
  clearFilters(): void {
    this.selectedCategoryName = '';
    this.selectedBrandName = '';
    this.priceRange = { min: this.minPrice, max: this.maxPrice };
    this.loadImages();
  }

  get paginatedImages(): any[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredImages.slice(start, start + this.itemsPerPage);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredImages.length / this.itemsPerPage);
  }

  nextPage(): void {
    if (this.currentPage * this.itemsPerPage < this.filteredImages.length) {
      this.currentPage++;
      window.scrollTo(0, 0);
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      window.scrollTo(0, 0);
    }
  }

  setViewMode(mode: string): void {
    this.viewMode = mode;
  }

  viewImage(image: any): void {
    this.selectedImage = image;
    document.body.style.overflow = 'hidden';
  }

  closeImageView(event: Event): void {
    if (
      event.target === event.currentTarget ||
      (event.target as Element).classList.contains('close-btn')
    ) {
      this.selectedImage = null;
      document.body.style.overflow = '';
    }
  }

  getCategoryName(categoryId: number): string {
    return this.categoriesMap.get(categoryId) || 'Unknown';
  }

  // Add method to get brand name
  getBrandName(brandId: number): string {
    return this.brandsMap.get(brandId) || 'Unknown';
  }

  getCategoryBackground(categoryName: string): string {
    const categoryMap: { [key: string]: string } = {
      'red': '#f44336',
      'blue': '#2196F3',
      'green': '#4CAF50',
      'yellow': '#FFEB3B',
      'purple': '#9C27B0',
      'orange': '#FF9800',
      'black': '#212121',
      'white': '#FFFFFF',
      'gray': '#9E9E9E',
      'pink': '#E91E63',
      'brown': '#795548',
      'cyan': '#00BCD4'
    };

    const lowerCaseName = categoryName.toLowerCase();
    for (const [key, value] of Object.entries(categoryMap)) {
      if (lowerCaseName.includes(key)) {
        return value;
      }
    }

    return '#6c757d';
  }

  getContrastCategory(categoryName: string): string {
    const bgCategory = this.getCategoryBackground(categoryName);
    const r = parseInt(bgCategory.slice(1, 3), 16);
    const g = parseInt(bgCategory.slice(3, 5), 16);
    const b = parseInt(bgCategory.slice(5, 7), 16);
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness > 125 ? '#000000' : '#FFFFFF';
  }
}
