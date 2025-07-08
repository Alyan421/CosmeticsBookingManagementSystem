import { Component, OnInit } from '@angular/core';
import { ImageService } from './image.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrandService } from '../Brands/brand.service';
import { CategoryService } from '../Categories/category.service';
import { ImageUploadComponent } from './image-upload.component';

@Component({
  selector: 'app-image-list',
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, ImageUploadComponent],
})
export class ImageListComponent implements OnInit {
  images: any[] = [];
  searchQuery: string = '';
  currentPage: number = 1;
  itemsPerPage: number = 30; // Updated to 30 images per page
  brands: any[] = [];
  categories: any[] = [];
  selectedBrandName: string = '';
  selectedCategoryName: string = '';
  brandImages: any[] = [];
  isDeleting: boolean = false;
  viewMode: string = 'grid'; // 'grid' or 'list'
  selectedImage: any = null;
  categoriesMap: Map<number, string> = new Map(); // Map categoryId to categoryName

  // New properties for update functionality
  showUpdateModal: boolean = false;
  imageToUpdate: any = null;

  constructor(
    private imageService: ImageService,
    private brandService: BrandService,
    private categoryService: CategoryService
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadBrands();
    this.loadCategories();
  }

  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((data) => {
      this.brands = data;
    });
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe((data) => {
      // Remove duplicates based on category name
      this.categories = data.filter(
        (category, index, self) =>
          index === self.findIndex((c) => c.categoryName === category.categoryName)
      );

      // Create a map of categoryId to categoryName for quick lookups
      data.forEach(category => {
        this.categoriesMap.set(category.id, category.categoryName);
      });
    });
  }

  filterByBrandName(): void {
    this.imageService.filterByBrandName(this.selectedBrandName).subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  filterByCategoryName(categoryName: string): void {
    this.selectedCategoryName = categoryName;
    this.imageService.filterByCategoryName(categoryName).subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  loadImages(): void {
    this.selectedCategoryName = '';
    this.imageService.getAllImages().subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  deleteImage(id: number): void {
    if (this.isDeleting) return; // Prevent multiple clicks

    this.isDeleting = true;

    if (confirm('Are you sure you want to delete this image?')) {
      this.imageService.deleteImage(id).subscribe({
        next: () => {
          this.loadImages(); // Reload the images after deletion
          this.isDeleting = false;
        },
        error: (error) => {
          console.error('Error deleting image:', error);
          alert('Failed to delete the image. Please try again.');
          this.isDeleting = false;
        }
      });
    } else {
      this.isDeleting = false;
    }
  }

  // New method to handle updating an image
  updateImage(image: any): void {
    this.imageToUpdate = image;
    this.showUpdateModal = true;
    document.body.style.overflow = 'hidden'; // Prevent scrolling
  }

  // Close the update modal
  closeUpdateModal(): void {
    this.showUpdateModal = false;
    this.imageToUpdate = null;
    document.body.style.overflow = ''; // Restore scrolling
  }

  // Handle update completion
  onUpdateComplete(): void {
    this.closeUpdateModal();
    this.loadImages(); // Refresh the image list
  }

  get paginatedImages(): any[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.images.slice(start, start + this.itemsPerPage);
  }

  get totalPages(): number {
    return Math.ceil(this.images.length / this.itemsPerPage);
  }

  nextPage(): void {
    if (this.currentPage * this.itemsPerPage < this.images.length) {
      this.currentPage++;
      window.scrollTo(0, 0); // Scroll to top when changing page
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      window.scrollTo(0, 0); // Scroll to top when changing page
    }
  }

  setViewMode(mode: string): void {
    this.viewMode = mode;
  }

  viewImage(image: any): void {
    this.selectedImage = image;
    document.body.style.overflow = 'hidden'; // Prevent scrolling
  }

  closeImageView(event: Event): void {
    if (
      event.target === event.currentTarget ||
      (event.target as Element).classList.contains('close-btn')
    ) {
      this.selectedImage = null;
      document.body.style.overflow = ''; // Restore scrolling
    }
  }

  getCategoryName(categoryId: number): string {
    return this.categoriesMap.get(categoryId) || 'Unknown';
  }

  getCategoryBackground(categoryName: string): string {
    // Map common category names to their hex values
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

    // Try to match the category name with our map
    const lowerCaseName = categoryName.toLowerCase();
    for (const [key, value] of Object.entries(categoryMap)) {
      if (lowerCaseName.includes(key)) {
        return value;
      }
    }

    // Default category if no match
    return '#6c757d';
  }

  getContrastCategory(categoryName: string): string {
    const bgCategory = this.getCategoryBackground(categoryName);

    // Convert hex to RGB
    const r = parseInt(bgCategory.slice(1, 3), 16);
    const g = parseInt(bgCategory.slice(3, 5), 16);
    const b = parseInt(bgCategory.slice(5, 7), 16);

    // Calculate brightness
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;

    // Return white for dark categories, black for light categories
    return brightness > 125 ? '#000000' : '#FFFFFF';
  }
}
