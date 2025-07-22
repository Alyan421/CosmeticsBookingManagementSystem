import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ImageService, ImageData } from './image.service';
import { BrandService } from '../Brands/brand.service';
import { CategoryService } from '../Categories/category.service';
import { ProductService, Product, ProductCreate, ProductUpdate } from '../Product/product.service';
import { NotificationService } from '../core/notification.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';

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
  products: Product[] = [];
  filteredProducts: Product[] = [];
  isLoading = false;
  isLoadingData = true;
  showCreateProduct = false;
  selectedProduct: Product | null = null;

  constructor(
    private fb: FormBuilder,
    private imageService: ImageService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private productService: ProductService,
    private notification: NotificationService,
    private router: Router
  ) {
    this.uploadForm = this.fb.group({
      file: [null, this.isUpdateMode ? null : Validators.required],
      productId: [''],
      brandId: ['', Validators.required],
      categoryId: ['', Validators.required],
      productName: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      availableProduct: [0, [Validators.required, Validators.min(0)]],
      price: [0, [Validators.required, Validators.min(0)]],
      createNewProduct: [false]
    });
  }

  ngOnInit(): void {
    this.loadData();
    this.setupFormWatchers();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageToUpdate'] && this.imageToUpdate) {
      this.prepopulateForm();
    }
  }

  setupFormWatchers(): void {
    // Watch for brand/category changes to filter products
    this.uploadForm.get('brandId')?.valueChanges.subscribe(() => {
      this.filterProducts();
      this.resetProductSelection();
      this.updateFormValidation();
    });

    this.uploadForm.get('categoryId')?.valueChanges.subscribe(() => {
      this.filterProducts();
      this.resetProductSelection();
      this.updateFormValidation();
    });

    // Watch for create new product toggle
    this.uploadForm.get('createNewProduct')?.valueChanges.subscribe((createNew: boolean) => {
      this.showCreateProduct = createNew;
      this.updateFormValidation();

      if (createNew) {
        this.enableProductFields();
        this.uploadForm.get('productId')?.clearValidators();
      } else {
        this.disableProductFields();
        this.uploadForm.get('productId')?.setValidators([Validators.required]);
      }
      this.uploadForm.get('productId')?.updateValueAndValidity();
    });

    // Watch for product selection
    this.uploadForm.get('productId')?.valueChanges.subscribe((productId: number) => {
      if (productId && !this.showCreateProduct) {
        const product = this.products.find(p => p.id === productId);
        if (product) {
          this.selectedProduct = product;
          this.populateProductDetails(product);
        }
      }
      this.updateFormValidation();
    });

    // Watch for file changes
    this.uploadForm.get('file')?.valueChanges.subscribe(() => {
      this.updateFormValidation();
    });
  }

  updateFormValidation(): void {
    // Update form validation based on current state
    const hasFile = !!this.uploadForm.get('file')?.value;
    const hasBrand = !!this.uploadForm.get('brandId')?.value;
    const hasCategory = !!this.uploadForm.get('categoryId')?.value;
    const hasProduct = !!this.uploadForm.get('productId')?.value;
    const isCreatingNew = this.uploadForm.get('createNewProduct')?.value;

    // Clear any existing validation state
    this.uploadForm.get('productId')?.markAsUntouched();
    this.uploadForm.get('productName')?.markAsUntouched();
    this.uploadForm.get('availableProduct')?.markAsUntouched();
    this.uploadForm.get('price')?.markAsUntouched();

    // Update validation
    if (isCreatingNew) {
      this.uploadForm.get('productId')?.clearValidators();
      this.uploadForm.get('productName')?.setValidators([Validators.required, Validators.maxLength(200)]);
      this.uploadForm.get('availableProduct')?.setValidators([Validators.required, Validators.min(0)]);
      this.uploadForm.get('price')?.setValidators([Validators.required, Validators.min(0)]);
    } else {
      this.uploadForm.get('productId')?.setValidators([Validators.required]);
      this.uploadForm.get('productName')?.clearValidators();
      this.uploadForm.get('availableProduct')?.clearValidators();
      this.uploadForm.get('price')?.clearValidators();
    }

    // Update form control states
    this.uploadForm.get('productId')?.updateValueAndValidity({ emitEvent: false });
    this.uploadForm.get('productName')?.updateValueAndValidity({ emitEvent: false });
    this.uploadForm.get('availableProduct')?.updateValueAndValidity({ emitEvent: false });
    this.uploadForm.get('price')?.updateValueAndValidity({ emitEvent: false });
  }

  loadData(): void {
    this.isLoadingData = true;

    forkJoin({
      brands: this.brandService.getAllBrands(),
      categories: this.categoryService.getAllCategories(),
      products: this.productService.getAllProduct()
    }).subscribe({
      next: (data) => {
        this.brands = data.brands;
        this.categories = data.categories;
        this.products = data.products;
        this.isLoadingData = false;

        if (this.isUpdateMode && this.imageToUpdate) {
          this.prepopulateForm();
        }
        this.updateFormValidation();
      },
      error: (error) => {
        this.notification.error('Failed to load data');
        console.error('Error loading data:', error);
        this.isLoadingData = false;
      }
    });
  }

  prepopulateForm(): void {
    if (!this.imageToUpdate) return;

    this.uploadForm.patchValue({
      productId: this.imageToUpdate.productId,
      brandId: this.imageToUpdate.brandId,
      categoryId: this.imageToUpdate.categoryId,
      productName: this.imageToUpdate.productName,
      description: this.imageToUpdate.productDescription || '',
      availableProduct: this.imageToUpdate.availableProduct,
      price: this.imageToUpdate.price,
      createNewProduct: false
    });

    this.imagePreview = this.imageToUpdate.url;
    this.selectedProduct = this.products.find(p => p.id === this.imageToUpdate!.productId) || null;
    this.filterProducts();
    this.updateFormValidation();
  }

  filterProducts(): void {
    const brandId = this.uploadForm.get('brandId')?.value;
    const categoryId = this.uploadForm.get('categoryId')?.value;

    if (brandId && categoryId) {
      // Filter products that don't have an image for the selected brand/category
      this.filteredProducts = this.products.filter(p =>
        p.brandId === parseInt(brandId) &&
        p.categoryId === parseInt(categoryId) &&
        !p.imageUrl // Only show products without images
      );
    } else if (brandId) {
      this.filteredProducts = this.products.filter(p =>
        p.brandId === parseInt(brandId) && !p.imageUrl
      );
    } else if (categoryId) {
      this.filteredProducts = this.products.filter(p =>
        p.categoryId === parseInt(categoryId) && !p.imageUrl
      );
    } else {
      this.filteredProducts = this.products.filter(p => !p.imageUrl);
    }
  }

  resetProductSelection(): void {
    this.uploadForm.patchValue({ productId: '' });
    this.selectedProduct = null;
    if (!this.showCreateProduct) {
      this.clearProductDetails();
    }
  }

  populateProductDetails(product: Product): void {
    this.uploadForm.patchValue({
      productName: product.productName,
      description: product.description || '',
      availableProduct: product.availableProduct,
      price: product.price
    });
  }

  clearProductDetails(): void {
    this.uploadForm.patchValue({
      productName: '',
      description: '',
      availableProduct: 0,
      price: 0
    });
  }

  enableProductFields(): void {
    ['productName', 'description', 'availableProduct', 'price'].forEach(field => {
      this.uploadForm.get(field)?.enable();
    });
  }

  disableProductFields(): void {
    ['productName', 'description', 'availableProduct', 'price'].forEach(field => {
      this.uploadForm.get(field)?.disable();
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
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
    this.updateFormValidation();
  }

  onSubmit(): void {
    if (this.uploadForm.invalid) {
      this.markFormGroupTouched(this.uploadForm);
      this.notification.error('Please fill in all required fields correctly');
      return;
    }

    this.isLoading = true;
    const formValues = this.uploadForm.value;

    if (this.showCreateProduct) {
      // Create new product first, then upload image
      this.createProductAndUploadImage(formValues);
    } else {
      // Use existing product
      this.uploadImageForExistingProduct(formValues);
    }
  }

  createProductAndUploadImage(formValues: any): void {
    const productData: ProductCreate = {
      brandId: parseInt(formValues.brandId),
      categoryId: parseInt(formValues.categoryId),
      productName: formValues.productName,
      description: formValues.description || undefined,
      availableProduct: formValues.availableProduct,
      price: formValues.price
    };

    this.productService.createProduct(productData).subscribe({
      next: (product) => {
        // Now upload image for the created product
        this.uploadImageForProduct(product.id, formValues.file);
      },
      error: (error) => {
        this.notification.error(error.error?.message || 'Failed to create product');
        console.error('Error creating product:', error);
        this.isLoading = false;
      }
    });
  }

  uploadImageForExistingProduct(formValues: any): void {
    if (this.isUpdateMode) {
      // Update existing product if details changed
      const productData: ProductUpdate = {
        id: formValues.productId,
        brandId: parseInt(formValues.brandId),
        categoryId: parseInt(formValues.categoryId),
        productName: formValues.productName,
        description: formValues.description || undefined,
        availableProduct: formValues.availableProduct,
        price: formValues.price
      };

      this.productService.updateProduct(productData).subscribe({
        next: () => {
          this.updateImageForProduct(formValues.productId, formValues.file);
        },
        error: (error) => {
          this.notification.error(error.error?.message || 'Failed to update product');
          console.error('Error updating product:', error);
          this.isLoading = false;
        }
      });
    } else {
      this.uploadImageForProduct(formValues.productId, formValues.file);
    }
  }

  uploadImageForProduct(productId: number, file: File): void {
    const formData = new FormData();
    formData.append('productId', productId.toString());
    formData.append('imageFile', file);

    this.imageService.uploadImage(formData).subscribe({
      next: (response) => {
        this.notification.success('Image uploaded successfully');
        this.isLoading = false;
        this.resetForm();
        this.router.navigate(['/images']);
      },
      error: (error) => {
        this.notification.error(error.error?.message || 'Failed to upload image');
        console.error('Error uploading image:', error);
        this.isLoading = false;
      }
    });
  }

  updateImageForProduct(productId: number, file: File | null): void {
    if (!this.imageToUpdate) return;

    const formData = new FormData();
    formData.append('id', this.imageToUpdate.id.toString());
    formData.append('productId', productId.toString());

    if (file) {
      formData.append('imageFile', file);
    }

    this.imageService.updateImage(this.imageToUpdate.id, formData).subscribe({
      next: () => {
        this.notification.success('Image updated successfully');
        this.isLoading = false;
        this.updateComplete.emit();
      },
      error: (error) => {
        this.notification.error(error.error?.message || 'Failed to update image');
        console.error('Error updating image:', error);
        this.isLoading = false;
      }
    });
  }

  resetForm(): void {
    this.uploadForm.reset({
      file: null,
      productId: '',
      brandId: '',
      categoryId: '',
      productName: '',
      description: '',
      availableProduct: 0,
      price: 0,
      createNewProduct: false
    });
    this.imagePreview = null;
    this.selectedProduct = null;
    this.showCreateProduct = false;
    this.filteredProducts = [...this.products.filter(p => !p.imageUrl)];
    this.updateFormValidation();
  }

  goBack(): void {
    if (this.isUpdateMode) {
      this.updateComplete.emit();
    } else {
      this.router.navigate(['/images']);
    }
  }

  markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  getBrandName(brandId: number): string {
    const brand = this.brands.find(b => b.id === brandId);
    return brand ? brand.name : 'Unknown';
  }

  getCategoryName(categoryId: number): string {
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.categoryName : 'Unknown';
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.uploadForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.uploadForm.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['maxlength']) return `${fieldName} is too long`;
      if (field.errors['min']) return `${fieldName} must be positive`;
    }
    return '';
  }
}
