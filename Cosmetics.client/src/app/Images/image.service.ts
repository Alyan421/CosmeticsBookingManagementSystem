import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

export interface ImageData {
  id: number;
  productId: number;
  url: string;
  productName: string;
  productDescription?: string;
  price: number;
  availableProduct: number;
  brandId: number;
  categoryId: number;
  brandName: string;
  categoryName: string;
}

export interface ImageCreateRequest {
  productId: number;
  imageFile: File;
}

export interface ImageUpdateRequest {
  id: number;
  productId: number;
}

// Backward compatibility interfaces
export interface ImageCreateByBrandCategoryRequest {
  brandId: number;
  categoryId: number;
  imageFile: File;
}

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private get baseUrl(): string {
    return `${environment.apiUrl}/Image`;
  }

  constructor(private http: HttpClient) { }

  // New product-centric methods
  getAllImages(): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}`);
  }

  getImageById(id: number): Observable<ImageData> {
    return this.http.get<ImageData>(`${this.baseUrl}/${id}`);
  }

  getImagesByProductId(productId: number): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}/product/${productId}`);
  }

  getImagesByCategory(categoryId: number): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}/category/${categoryId}`);
  }

  getImagesByBrand(brandId: number): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}/brand/${brandId}`);
  }

  uploadImage(formData: FormData): Observable<ImageData> {
    return this.http.post<ImageData>(`${this.baseUrl}/upload`, formData);
  }

  updateImage(id: number, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, formData);
  }

  deleteImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  filterByBrandName(brandName: string): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}/filter-by-brand-name/${brandName}`);
  }

  filterByCategoryName(categoryName: string): Observable<ImageData[]> {
    return this.http.get<ImageData[]>(`${this.baseUrl}/filter-by-category/${categoryName}`);
  }

  // Backward compatibility methods (deprecated)
  /** @deprecated Use getImagesByCategory instead */
  getImagesByCategoryId(categoryId: number): Observable<ImageData[]> {
    return this.getImagesByCategory(categoryId);
  }

  /** @deprecated Use uploadImage with ProductId instead */
  uploadImageByBrandCategory(formData: FormData): Observable<ImageData> {
    return this.http.post<ImageData>(`${this.baseUrl}/upload-by-brand-category`, formData);
  }

  /** @deprecated Use updateImage with ProductId instead */
  updateImageByBrandCategory(id: number, formData: FormData): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/update-by-brand-category/${id}`, formData);
  }
}
