import { Component, OnInit } from '@angular/core';
import { ImageService } from './image.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClothService } from '../Cloths/cloth.service';
import { ColorService } from '../Colors/color.service';
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
  cloths: any[] = [];
  colors: any[] = [];
  selectedClothName: string = '';
  selectedColorName: string = '';
  clothImages: any[] = [];
  isDeleting: boolean = false;
  viewMode: string = 'grid'; // 'grid' or 'list'
  selectedImage: any = null;
  colorsMap: Map<number, string> = new Map(); // Map colorId to colorName

  // New properties for update functionality
  showUpdateModal: boolean = false;
  imageToUpdate: any = null;

  constructor(
    private imageService: ImageService,
    private clothService: ClothService,
    private colorService: ColorService
  ) { }

  ngOnInit(): void {
    this.loadImages();
    this.loadCloths();
    this.loadColors();
  }

  loadCloths(): void {
    this.clothService.getAllCloths().subscribe((data) => {
      this.cloths = data;
    });
  }

  loadColors(): void {
    this.colorService.getAllColors().subscribe((data) => {
      // Remove duplicates based on color name
      this.colors = data.filter(
        (color, index, self) =>
          index === self.findIndex((c) => c.colorName === color.colorName)
      );

      // Create a map of colorId to colorName for quick lookups
      data.forEach(color => {
        this.colorsMap.set(color.id, color.colorName);
      });
    });
  }

  filterByClothName(): void {
    this.imageService.filterByClothName(this.selectedClothName).subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  filterByColorName(colorName: string): void {
    this.selectedColorName = colorName;
    this.imageService.filterByColorName(colorName).subscribe((data) => {
      this.images = data;
      this.currentPage = 1; // Reset to the first page
    });
  }

  loadImages(): void {
    this.selectedColorName = '';
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

  getColorName(colorId: number): string {
    return this.colorsMap.get(colorId) || 'Unknown';
  }

  getColorBackground(colorName: string): string {
    // Map common color names to their hex values
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

    // Try to match the color name with our map
    const lowerCaseName = colorName.toLowerCase();
    for (const [key, value] of Object.entries(colorMap)) {
      if (lowerCaseName.includes(key)) {
        return value;
      }
    }

    // Default color if no match
    return '#6c757d';
  }

  getContrastColor(colorName: string): string {
    const bgColor = this.getColorBackground(colorName);

    // Convert hex to RGB
    const r = parseInt(bgColor.slice(1, 3), 16);
    const g = parseInt(bgColor.slice(3, 5), 16);
    const b = parseInt(bgColor.slice(5, 7), 16);

    // Calculate brightness
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;

    // Return white for dark colors, black for light colors
    return brightness > 125 ? '#000000' : '#FFFFFF';
  }
}
