import { Component, OnInit } from '@angular/core';
import { ColorService } from './color.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NotificationService } from '../core/notification.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-color-list',
  templateUrl: './color-list.component.html',
  styleUrls: ['./color-list.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ]
})
export class ColorListComponent implements OnInit {
  colors: any[] = [];
  colorForm: FormGroup;
  isEditing = false;
  selectedColorId: number | null = null;
  isLoading = true;
  searchTerm: string = '';
  filteredColors: any[] = [];

  constructor(
    private colorService: ColorService,
    private fb: FormBuilder,
    private notification: NotificationService
  ) {
    this.colorForm = this.fb.group({
      colorName: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.isLoading = true;
    this.colorService.getAllColors().subscribe({
      next: (data) => {
        this.colors = data;
        this.filterColors();
        this.isLoading = false;
      },
      error: (error) => {
        this.notification.error('Failed to load colors');
        console.error('Error loading colors:', error);
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.colorForm.invalid) {
      this.notification.error('Please fill in all required fields');
      return;
    }

    const colorData = this.colorForm.value;

    if (this.isEditing && this.selectedColorId) {
      // Update color
      colorData.id = this.selectedColorId;
      this.colorService.updateColor(colorData).subscribe({
        next: (response) => {
          this.notification.success('Color updated successfully');
          this.resetForm();
          this.loadColors();
        },
        error: (error) => {
          this.notification.error('Failed to update color');
          console.error('Error updating color:', error);
        }
      });
    } else {
      // Create color
      this.colorService.createColor(colorData).subscribe({
        next: (response) => {
          this.notification.success('Color created successfully');
          this.resetForm();
          this.loadColors();
        },
        error: (error) => {
          this.notification.error('Failed to create color');
          console.error('Error creating color:', error);
        }
      });
    }
  }

  editColor(color: any): void {
    this.isEditing = true;
    this.selectedColorId = color.id;

    this.colorForm.patchValue({
      colorName: color.colorName
    });
  }

  deleteColor(colorId: number): void {
    if (confirm('Are you sure you want to delete this color?')) {
      this.colorService.deleteColor(colorId).subscribe({
        next: () => {
          this.notification.success('Color deleted successfully');
          this.loadColors();
          if (this.selectedColorId === colorId) {
            this.resetForm();
          }
        },
        error: (error) => {
          this.notification.error('Failed to delete color');
          console.error('Error deleting color:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.isEditing = false;
    this.selectedColorId = null;
    this.colorForm.reset({
      colorName: ''
    });
  }

  onSearch(): void {
    this.filterColors();
  }

  filterColors(): void {
    if (!this.searchTerm) {
      this.filteredColors = this.colors;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredColors = this.colors.filter(color =>
        color.colorName.toLowerCase().includes(term)
      );
    }
  }

  // Navigate to stock management for this color
  navigateToStockManagement(colorId: number): void {
    // This will be handled by the router link in the template
  }
}
