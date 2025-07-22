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
  allImages: any[] = []; // Store all images for filtering
  filteredImages: any[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 30;
  categories: any[] = [];
  brands: any[] = [];
  selectedCategoryName: string = '';
  selectedBrandName: string = '';
  viewMode: string = 'grid';
  selectedImage: any = null;
  categoriesMap: Map<number, string> = new Map();
  brandsMap: Map<number, string> = new Map();
  currency: string = 'PKR';

  // Price filter properties
  minPrice: number = 0;
  maxPrice: number = 1000;
  priceRange: { min: number, max: number } = { min: 0, max: 1000 };
  isFilterActive: boolean = false;

  constructor(
    private imageService: ImageService,
    private categoryService: CategoryService,
    private brandService: BrandService
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadCategories();
    this.loadBrands();
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

  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((data) => {
      this.brands = data;

      // Create a map of brandId to brandName for quick lookups
      data.forEach(brand => {
        this.brandsMap.set(brand.id, brand.name);
      });
    });
  }

  loadImages(): void {
    this.imageService.getAllImages().subscribe((data) => {
      this.allImages = data; // Store all images
      this.images = [...data]; // Copy for current display

      // Set price range based on actual data
      if (data.length > 0) {
        const prices = data.map(img => img.price).filter(price => price !== undefined && price > 0);
        if (prices.length > 0) {
          this.minPrice = Math.floor(Math.min(...prices));
          this.maxPrice = Math.ceil(Math.max(...prices));
          this.priceRange = { min: this.minPrice, max: this.maxPrice };
        }
      }

      this.applyAllFilters();
      this.currentPage = 1;
    });
  }

  // Unified filter application method
  applyAllFilters(): void {
    let filtered = [...this.allImages];

    // Apply category filter
    if (this.selectedCategoryName && this.selectedCategoryName !== '') {
      filtered = filtered.filter(image =>
        this.getCategoryName(image.categoryId).toLowerCase() === this.selectedCategoryName.toLowerCase()
      );
    }

    // Apply brand filter
    if (this.selectedBrandName && this.selectedBrandName !== '') {
      filtered = filtered.filter(image =>
        this.getBrandName(image.brandId).toLowerCase() === this.selectedBrandName.toLowerCase()
      );
    }

    // Apply price filter
    filtered = filtered.filter(image => {
      return image.price >= this.priceRange.min && image.price <= this.priceRange.max;
    });

    this.filteredImages = filtered;

    // Check if any filter is active
    this.isFilterActive = this.selectedCategoryName !== '' ||
      this.selectedBrandName !== '' ||
      this.priceRange.min > this.minPrice ||
      this.priceRange.max < this.maxPrice;

    this.currentPage = 1;
  }

  // Updated filter methods to use unified filtering
  filterByCategoryName(categoryName: string): void {
    this.selectedCategoryName = categoryName;
    this.applyAllFilters();
  }

  filterByBrandName(brandName: string): void {
    this.selectedBrandName = brandName;
    this.applyAllFilters();
  }

  onCategoryFilterChange(): void {
    this.applyAllFilters();
  }

  onBrandFilterChange(): void {
    this.applyAllFilters();
  }

  onPriceFilterChange(): void {
    this.applyAllFilters();
  }

  clearFilters(): void {
    this.selectedCategoryName = '';
    this.selectedBrandName = '';
    this.priceRange = { min: this.minPrice, max: this.maxPrice };
    this.applyAllFilters();
  }

  // Reset specific filters while maintaining others
  clearCategoryFilter(): void {
    this.selectedCategoryName = '';
    this.applyAllFilters();
  }

  clearBrandFilter(): void {
    this.selectedBrandName = '';
    this.applyAllFilters();
  }

  clearPriceFilter(): void {
    this.priceRange = { min: this.minPrice, max: this.maxPrice };
    this.applyAllFilters();
  }

  // Search functionality (if needed)
  searchImages(searchTerm: string): void {
    if (!searchTerm || searchTerm.trim() === '') {
      this.applyAllFilters();
      return;
    }

    const term = searchTerm.toLowerCase();
    let filtered = [...this.allImages];

    // Apply existing filters first
    if (this.selectedCategoryName && this.selectedCategoryName !== '') {
      filtered = filtered.filter(image =>
        this.getCategoryName(image.categoryId).toLowerCase() === this.selectedCategoryName.toLowerCase()
      );
    }

    if (this.selectedBrandName && this.selectedBrandName !== '') {
      filtered = filtered.filter(image =>
        this.getBrandName(image.brandId).toLowerCase() === this.selectedBrandName.toLowerCase()
      );
    }

    filtered = filtered.filter(image => {
      return image.price >= this.priceRange.min && image.price <= this.priceRange.max;
    });

    // Apply search filter
    filtered = filtered.filter(image =>
      (image.productName && image.productName.toLowerCase().includes(term)) ||
      (image.productDescription && image.productDescription.toLowerCase().includes(term)) ||
      this.getCategoryName(image.categoryId).toLowerCase().includes(term) ||
      this.getBrandName(image.brandId).toLowerCase().includes(term)
    );

    this.filteredImages = filtered;
    this.currentPage = 1;
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

  getCategoryIcon(categoryName: string): string {
    // Map category names to icon images
    const categoryIcons: { [key: string]: string } = {
      'Lipstick': 'assets/icons/lipstick.png',
      'Mascara': 'assets/icons/mascara.png',
      'Eyeshadow': 'assets/icons/eyeshadow.png',
      'Foundation': 'assets/icons/foundation.png',
      'Blush': 'assets/icons/blush.png',
      'Nail Polish': 'assets/icons/nail-polish.png',
      'Skincare': 'assets/icons/skincare.png',
      'Fragrance': 'assets/icons/fragrance.png',
      'Brushes': 'assets/icons/brushes.png',
      'Eyeliner': 'assets/icons/eyeliner.png',
      'Powder': 'assets/icons/powder.png',
      'Palette': 'assets/icons/palette.png'
    };

    // If we have a direct match, return the icon
    if (categoryName && categoryIcons[categoryName]) {
      return categoryIcons[categoryName];
    }

    // Check for partial matches in the category name
    if (categoryName) {
      const lowerCaseName = categoryName.toLowerCase();
      for (const [key, iconPath] of Object.entries(categoryIcons)) {
        if (lowerCaseName.includes(key.toLowerCase())) {
          return iconPath;
        }
      }
    }

    // Default fallback icon
    return 'assets/icons/cosmetics-default.png';
  }

  // Additional utility methods for filter status
  getActiveFiltersCount(): number {
    let count = 0;
    if (this.selectedCategoryName !== '') count++;
    if (this.selectedBrandName !== '') count++;
    if (this.priceRange.min > this.minPrice || this.priceRange.max < this.maxPrice) count++;
    return count;
  }

  getFilterSummary(): string {
    const filters: string[] = [];

    if (this.selectedCategoryName !== '') {
      filters.push(`Category: ${this.selectedCategoryName}`);
    }

    if (this.selectedBrandName !== '') {
      filters.push(`Brand: ${this.selectedBrandName}`);
    }

    if (this.priceRange.min > this.minPrice || this.priceRange.max < this.maxPrice) {
      filters.push(`Price: ${this.priceRange.min} - ${this.priceRange.max} ${this.currency}`);
    }

    return filters.length > 0 ? filters.join(', ') : 'No filters applied';
  }
}
