import { Component, OnInit } from '@angular/core';
import { ImageService } from '../Images/image.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ColorService } from '../Colors/color.service';
import { ClothService } from '../Cloths/cloth.service';

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
  colors: any[] = [];
  cloths: any[] = []; // Add cloths array
  selectedColorName: string = '';
  selectedClothName: string = ''; // Add selected cloth name
  viewMode: string = 'grid';
  selectedImage: any = null;
  colorsMap: Map<number, string> = new Map();
  clothsMap: Map<number, string> = new Map(); // Add cloths map
  currency: string = 'PKR'; // Set currency to PKR

  // Price filter properties
  minPrice: number = 0;
  maxPrice: number = 1000;
  priceRange: { min: number, max: number } = { min: 0, max: 1000 };
  isFilterActive: boolean = false;

  constructor(
    private imageService: ImageService,
    private colorService: ColorService,
    private clothService: ClothService // Add cloth service
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadColors();
    this.loadCloths(); // Load cloths on init
  }

  loadColors(): void {
    this.colorService.getAllColors().subscribe((data) => {
      this.colors = data.filter(
        (color, index, self) =>
          index === self.findIndex((c) => c.colorName === color.colorName)
      );

      data.forEach(color => {
        this.colorsMap.set(color.id, color.colorName);
      });
    });
  }

  // Add method to load cloths
  loadCloths(): void {
    this.clothService.getAllCloths().subscribe((data) => {
      this.cloths = data;

      // Create a map of clothId to clothName for quick lookups
      data.forEach(cloth => {
        this.clothsMap.set(cloth.id, cloth.name);
      });
    });
  }

  filterByColorName(colorName: string): void {
    this.selectedColorName = colorName;
    this.imageService.filterByColorName(colorName).subscribe((data) => {
      this.images = data;
      this.applyFilters();
      this.currentPage = 1;
    });
  }

  // Add method to filter by cloth name
  filterByClothName(clothName: string): void {
    this.selectedClothName = clothName;
    this.imageService.filterByClothName(clothName).subscribe((data) => {
      this.images = data;
      this.applyFilters();
      this.currentPage = 1;
    });
  }

  loadImages(): void {
    this.selectedColorName = '';
    this.selectedClothName = '';
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
      this.isFilterActive = this.selectedColorName !== '' ||
        this.selectedClothName !== '' || // Add cloth name to filter check
        this.priceRange.min > this.minPrice ||
        this.priceRange.max < this.maxPrice;

      return priceInRange;
    });
    this.currentPage = 1;
  }

  onColorFilterChange(): void {
    if (this.selectedColorName) {
      this.filterByColorName(this.selectedColorName);
    } else {
      // When "All Colors" is selected (empty value)
      this.loadImages();
    }
  }

  // Add method for cloth filter change
  onClothFilterChange(): void {
    if (this.selectedClothName) {
      this.filterByClothName(this.selectedClothName);
    } else {
      // When "All Cloths" is selected (empty value)
      this.loadImages();
    }
  }

  onPriceFilterChange(): void {
    this.applyFilters();
  }

  // Update clear filters to include cloth filter
  clearFilters(): void {
    this.selectedColorName = '';
    this.selectedClothName = '';
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

  getColorName(colorId: number): string {
    return this.colorsMap.get(colorId) || 'Unknown';
  }

  // Add method to get cloth name
  getClothName(clothId: number): string {
    return this.clothsMap.get(clothId) || 'Unknown';
  }

  getColorBackground(colorName: string): string {
    const colorMap: { [key: string]: string } = {
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

    const lowerCaseName = colorName.toLowerCase();
    for (const [key, value] of Object.entries(colorMap)) {
      if (lowerCaseName.includes(key)) {
        return value;
      }
    }

    return '#6c757d';
  }

  getContrastColor(colorName: string): string {
    const bgColor = this.getColorBackground(colorName);
    const r = parseInt(bgColor.slice(1, 3), 16);
    const g = parseInt(bgColor.slice(3, 5), 16);
    const b = parseInt(bgColor.slice(5, 7), 16);
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness > 125 ? '#000000' : '#FFFFFF';
  }
}
