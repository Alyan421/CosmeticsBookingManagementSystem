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
    // Initialize the form with the correct order of properties
    this.brandForm = this.fb.group({
      id: [null],
      description: [''],
      name: [''],
      price: ['']
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
      // Make sure the form is updated with the correct order
      this.brandForm.patchValue({
        id: data.id,
        description: data.description,
        name: data.name,
        price: data.price
      });
    });
  }

  addBrand(): void {
    // Create a properly ordered object for API submission
    const brandData = {
      description: this.brandForm.value.description,
      name: this.brandForm.value.name,
      price: this.brandForm.value.price
    };

    this.brandService.createBrand(brandData).subscribe(() => {
      this.loadBrands();
      this.brandForm.reset();
    });
  }

  updateBrand(): void {
    const id = this.brandForm.value.id;
    // Create a properly ordered object for API submission
    const brandData = {
      id: id,
      description: this.brandForm.value.description,
      name: this.brandForm.value.name,
      price: this.brandForm.value.price
    };

    this.brandService.updateBrand(brandData).subscribe(() => {
      this.loadBrands();
      this.brandForm.reset();
    });
  }

  editBrand(brand: any): void {
    this.selectedBrand = brand;
    // Make sure the form is updated with the correct order
    this.brandForm.patchValue({
      id: brand.id,
      description: brand.description,
      name: brand.name,
      price: brand.price
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

  // New method for search button click
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
