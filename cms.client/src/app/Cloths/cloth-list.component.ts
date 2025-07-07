import { Component, OnInit } from '@angular/core';
import { ClothService } from './cloth.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cloth-list',
  templateUrl: './cloth-list.component.html',
  styleUrls: ['./cloth-list.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
})
export class ClothListComponent implements OnInit {
  cloths: any[] = [];
  selectedCloth: any = null;
  clothForm: FormGroup;
  filteredCloths: any[] = [];
  searchTerm: string = '';

  constructor(private clothService: ClothService, private fb: FormBuilder) {
    // Initialize the form with the correct order of properties
    this.clothForm = this.fb.group({
      id: [null],
      description: [''],
      name: [''],
      price: ['']
    });
  }

  ngOnInit(): void {
    this.loadCloths();
  }

  loadCloths(): void {
    this.clothService.getAllCloths().subscribe((data) => {
      this.cloths = data;
      this.filteredCloths = data; // Initialize filtered list
    });
  }

  getClothById(id: number): void {
    this.clothService.getClothById(id).subscribe(data => {
      this.selectedCloth = data;
      // Make sure the form is updated with the correct order
      this.clothForm.patchValue({
        id: data.id,
        description: data.description,
        name: data.name,
        price: data.price
      });
    });
  }

  addCloth(): void {
    // Create a properly ordered object for API submission
    const clothData = {
      description: this.clothForm.value.description,
      name: this.clothForm.value.name,
      price: this.clothForm.value.price
    };

    this.clothService.createCloth(clothData).subscribe(() => {
      this.loadCloths();
      this.clothForm.reset();
    });
  }

  updateCloth(): void {
    const id = this.clothForm.value.id;
    // Create a properly ordered object for API submission
    const clothData = {
      id: id,
      description: this.clothForm.value.description,
      name: this.clothForm.value.name,
      price: this.clothForm.value.price
    };

    this.clothService.updateCloth(clothData).subscribe(() => {
      this.loadCloths();
      this.clothForm.reset();
    });
  }

  editCloth(cloth: any): void {
    this.selectedCloth = cloth;
    // Make sure the form is updated with the correct order
    this.clothForm.patchValue({
      id: cloth.id,
      description: cloth.description,
      name: cloth.name,
      price: cloth.price
    });
  }

  deleteCloth(id: number): void {
    if (confirm('Are you sure you want to delete this cloth?')) {
      this.clothService.deleteCloth(id).subscribe(() => {
        this.loadCloths();
      });
    }
  }

  searchCloths(event: any): void {
    this.searchTerm = event.target.value.toLowerCase();
    this.applySearch();
  }

  // New method for search button click
  searchClothsButton(): void {
    this.applySearch();
  }

  // Common search functionality
  private applySearch(): void {
    if (this.searchTerm) {
      this.filteredCloths = this.cloths.filter(cloth =>
        cloth.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        cloth.description.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    } else {
      this.filteredCloths = this.cloths;
    }
  }
}
