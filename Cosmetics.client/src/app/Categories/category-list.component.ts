import { Component, OnInit } from '@angular/core';
import { CategoryService } from './category.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NotificationService } from '../core/notification.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ]
})
export class CategoryListComponent implements OnInit {
  categories: any[] = [];
  categoryForm: FormGroup;
  isEditing = false;
  selectedCategoryId: number | null = null;
  isLoading = true;
  searchTerm: string = '';
  filteredCategories: any[] = [];

  constructor(
    private categoryService: CategoryService,
    private fb: FormBuilder,
    private notification: NotificationService
  ) {
    this.categoryForm = this.fb.group({
      categoryName: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (data) => {
        this.categories = data;
        this.filterCategories();
        this.isLoading = false;
      },
      error: (error) => {
        this.notification.error('Failed to load categories');
        console.error('Error loading categories:', error);
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.notification.error('Please fill in all required fields');
      return;
    }

    const categoryData = this.categoryForm.value;

    if (this.isEditing && this.selectedCategoryId) {
      // Update category
      categoryData.id = this.selectedCategoryId;
      this.categoryService.updateCategory(categoryData).subscribe({
        next: (response) => {
          this.notification.success('Category updated successfully');
          this.resetForm();
          this.loadCategories();
        },
        error: (error) => {
          this.notification.error('Failed to update category');
          console.error('Error updating category:', error);
        }
      });
    } else {
      // Create category
      this.categoryService.createCategory(categoryData).subscribe({
        next: (response) => {
          this.notification.success('Category created successfully');
          this.resetForm();
          this.loadCategories();
        },
        error: (error) => {
          this.notification.error('Failed to create category');
          console.error('Error creating category:', error);
        }
      });
    }
  }

  editCategory(category: any): void {
    this.isEditing = true;
    this.selectedCategoryId = category.id;

    this.categoryForm.patchValue({
      categoryName: category.categoryName
    });
  }

  deleteCategory(categoryId: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(categoryId).subscribe({
        next: () => {
          this.notification.success('Category deleted successfully');
          this.loadCategories();
          if (this.selectedCategoryId === categoryId) {
            this.resetForm();
          }
        },
        error: (error) => {
          this.notification.error('Failed to delete category');
          console.error('Error deleting category:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.isEditing = false;
    this.selectedCategoryId = null;
    this.categoryForm.reset({
      categoryName: ''
    });
  }

  onSearch(): void {
    this.filterCategories();
  }

  filterCategories(): void {
    if (!this.searchTerm) {
      this.filteredCategories = this.categories;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredCategories = this.categories.filter(category =>
        category.categoryName.toLowerCase().includes(term)
      );
    }
  }

  // Navigate to stock management for this category
  navigateToStockManagement(categoryId: number): void {
    // This will be handled by the router link in the template
  }
}
