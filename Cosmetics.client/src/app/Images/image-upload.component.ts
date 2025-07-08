import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ImageService } from './image.service';
import { BrandService } from '../Brands/brand.service';
import { CategoryService } from '../Categories/category.service';
import { NotificationService } from '../core/notification.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { StockService, Stock, StockUpdate } from '../Stock/stock.service';

interface ImageData {
  id: number;
  brandId: number;
  categoryId: number;
  url: string;
}

@Component({
  selector: 'app-image-upload',
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class ImageUploadComponent implements OnInit, OnChanges {
  @Input() isUpdateMode = false;
  @Input() imageToUpdate: ImageData | null = null;
  @Output() updateComplete = new EventEmitter<void>();

  uploadForm: FormGroup;
  imagePreview: string | null = null;
  brands: any[] = [];
  categories: any[] = [];
  isLoading = false;
  isLoadingData = true;

  constructor(
    private fb: FormBuilder,
    private imageService: ImageService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private stockService: StockService,
    private notification: NotificationService,
    private router: Router
  ) {
    this.uploadForm = this.fb.group({
      file: [null, this.isUpdateMode ? null : Validators.required],
      brandId: ['', Validators.required],
      categoryId: ['', Validators.required],
      availableStock: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageToUpdate'] && this.imageToUpdate) {
      this.prepopulateForm();
    }
  }

  prepopulateForm(): void {
    if (!this.imageToUpdate) return;

    // Fetch stock data to get available stock
    this.stockService.getStock(
      this.imageToUpdate.brandId,
      this.imageToUpdate.categoryId
    ).subscribe({
      next: (stockData: Stock) => {
        this.uploadForm.patchValue({
          brandId: this.imageToUpdate?.brandId,
          categoryId: this.imageToUpdate?.categoryId,
          availableStock: stockData?.availableStock || 0
        });

        // Show the current image preview - ensure we handle undefined
        this.imagePreview = this.imageToUpdate?.url || null;
      },
      error: (error: any) => {
        console.error('Error fetching stock data:', error);
        // Still set the brand and category IDs even if stock fetch fails
        this.uploadForm.patchValue({
          brandId: this.imageToUpdate?.brandId,
          categoryId: this.imageToUpdate?.categoryId
        });
        this.imagePreview = this.imageToUpdate?.url || null;
      }
    });
  }

  loadData(): void {
    this.isLoadingData = true;

    // Load brands
    this.brandService.getAllBrands().subscribe({
      next: (data) => {
        this.brands = data;
        // Load categories
        this.categoryService.getAllCategories().subscribe({
          next: (categoryData) => {
            this.categories = categoryData;
            this.isLoadingData = false;

            // If in update mode, prepopulate the form
            if (this.isUpdateMode && this.imageToUpdate) {
              this.prepopulateForm();
            }
          },
          error: (error: any) => {
            this.notification.error('Failed to load categories');
            console.error('Error loading categories:', error);
            this.isLoadingData = false;
          }
        });
      },
      error: (error: any) => {
        this.notification.error('Failed to load brands');
        console.error('Error loading brands:', error);
        this.isLoadingData = false;
      }
    });
  }

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      const file = input.files[0];
      this.uploadForm.patchValue({ file });

      // Preview the image
      const reader = new FileReader();
      reader.onload = () => {
        // Ensure we're handling the correct type
        this.imagePreview = reader.result ? (reader.result as string) : null;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (this.uploadForm.invalid) {
      this.notification.error('Please fill in all required fields');
      return;
    }

    this.isLoading = true;
    const formData = new FormData();
    const formValues = this.uploadForm.value;

    formData.append('brandId', formValues.brandId);
    formData.append('categoryId', formValues.categoryId);

    // Only append file if it was selected (required for new uploads, optional for updates)
    if (formValues.file) {
      formData.append('imagefile', formValues.file);
    }

    // First create/update the stock entry
    const stockData: StockUpdate = {
      brandId: formValues.brandId,
      categoryId: formValues.categoryId,
      availableStock: formValues.availableStock
    };

    this.stockService.updateStock(stockData).subscribe({
      next: (stockResponse: Stock) => {
        // Now upload/update the image
        if (this.isUpdateMode && this.imageToUpdate) {
          // Update existing image
          this.imageService.updateImage(this.imageToUpdate.id, formData).subscribe({
            next: (response) => {
              this.notification.success('Image updated successfully');
              this.isLoading = false;

              if (this.isUpdateMode) {
                this.updateComplete.emit();
              } else {
                this.resetForm();
                this.router.navigate(['/images']);
              }
            },
            error: (error: any) => {
              this.notification.error('Failed to update image');
              console.error('Error updating image:', error);
              this.isLoading = false;
            }
          });
        } else {
          // Upload new image
          this.imageService.uploadImage(formData).subscribe({
            next: (response) => {
              this.notification.success('Image uploaded and stock created successfully');
              this.resetForm();
              this.router.navigate(['/images']);
            },
            error: (error: any) => {
              this.notification.error('Failed to upload image');
              console.error('Error uploading image:', error);
              this.isLoading = false;
            }
          });
        }
      },
      error: (error: any) => {
        this.notification.error('Failed to update stock');
        console.error('Error updating stock:', error);
        this.isLoading = false;
      }
    });
  }

  resetForm(): void {
    this.uploadForm.reset({
      file: null,
      brandId: '',
      categoryId: '',
      availableStock: 0
    });
    this.imagePreview = null;
  }

  goBack(): void {
    if (this.isUpdateMode) {
      this.updateComplete.emit();
    } else {
      this.router.navigate(['/images']);
    }
  }
}
