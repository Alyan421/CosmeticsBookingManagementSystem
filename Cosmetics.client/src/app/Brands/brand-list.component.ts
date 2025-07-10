import { Component, OnInit } from '@angular/core';
import { BrandService } from './brand.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-brand-list',
  templateUrl: './brand-list.component.html',
  styleUrls: ['./brand-list.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
})
export class BrandListComponent implements OnInit {
  brands: any[] = [];
  selectedBrand: any = null;
  brandForm: FormGroup;
  filteredBrands: any[] = [];
  searchTerm: string = '';

  constructor(private brandService: BrandService, private fb: FormBuilder) {
    // Initialize the form with updated properties (no price)
    this.brandForm = this.fb.group({
      id: [null],
      name: [''],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((data) => {
      this.brands = data;
      this.filteredBrands = data; // Initialize filtered list
    });
  }

  getBrandById(id: number): void {
    this.brandService.getBrandById(id).subscribe(data => {
      this.selectedBrand = data;
      // Update form with brand data (no price)
      this.brandForm.patchValue({
        id: data.id,
        name: data.name,
        description: data.description
      });
    });
  }

  addBrand(): void {
    // Create a brand object for API submission (no price)
    const brandData = {
      name: this.brandForm.value.name,
      description: this.brandForm.value.description
    };

    this.brandService.createBrand(brandData).subscribe(() => {
      this.loadBrands();
      this.brandForm.reset();
    });
  }

  updateBrand(): void {
    const id = this.brandForm.value.id;
    // Create a brand object for API submission (no price)
    const brandData = {
      id: id,
      name: this.brandForm.value.name,
      description: this.brandForm.value.description
    };

    this.brandService.updateBrand(brandData).subscribe(() => {
      this.loadBrands();
      this.brandForm.reset();
    });
  }

  editBrand(brand: any): void {
    this.selectedBrand = brand;
    // Update form with brand data (no price)
    this.brandForm.patchValue({
      id: brand.id,
      name: brand.name,
      description: brand.description
    });
  }

  deleteBrand(id: number): void {
    if (confirm('Are you sure you want to delete this brand?')) {
      this.brandService.deleteBrand(id).subscribe(() => {
        this.loadBrands();
      });
    }
  }

  searchBrands(event: any): void {
    this.searchTerm = event.target.value.toLowerCase();
    this.applySearch();
  }

  // Method for search button click
  searchBrandsButton(): void {
    this.applySearch();
  }

  // Common search functionality
  private applySearch(): void {
    if (this.searchTerm) {
      this.filteredBrands = this.brands.filter(brand =>
        brand.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        brand.description.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    } else {
      this.filteredBrands = this.brands;
    }
  }
}
