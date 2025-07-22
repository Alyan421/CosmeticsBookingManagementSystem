import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

export interface Product {
  id: number;
  brandId: number;
  categoryId: number;
  brandName: string;
  categoryName: string;
  productName: string;
  description?: string;
  availableProduct: number;
  price: number;
  imageUrl?: string;
}

export interface ProductCreate {
  brandId: number;
  categoryId: number;
  productName: string;
  description?: string;
  availableProduct: number;
  price: number;
}

export interface ProductUpdate {
  id: number;
  brandId: number;
  categoryId: number;
  productName: string;
  description?: string;
  availableProduct: number;
  price: number;
}

export interface ProductByBrandCategory {
  brandId: number;
  categoryId: number;
  brandName: string;
  categoryName: string;
  products: ProductSummary[];
}

export interface ProductSummary {
  id: number;
  productName: string;
  description?: string;
  availableProduct: number;
  price: number;
  imageUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private get baseUrl(): string {
    return `${environment.apiUrl}/Product`;
  }

  constructor(private http: HttpClient) { }

  getAllProduct(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}`);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  getProductsByBrand(brandId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/brand/${brandId}`);
  }

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/category/${categoryId}`);
  }

  getProductsByBrandAndCategory(brandId: number, categoryId: number): Observable<ProductByBrandCategory> {
    return this.http.get<ProductByBrandCategory>(`${this.baseUrl}/brand/${brandId}/category/${categoryId}`);
  }

  createProduct(product: ProductCreate): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}`, product);
  }

  updateProduct(product: ProductUpdate): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/${product.id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  // Backward compatibility methods (deprecated)
  /** @deprecated Use getProductsByBrandAndCategory instead */
  getProductByBrand(brandId: number): Observable<Product[]> {
    return this.getProductsByBrand(brandId);
  }

  /** @deprecated Use getProductsByCategory instead */
  getProductByCategory(categoryId: number): Observable<Product[]> {
    return this.getProductsByCategory(categoryId);
  }

  /** @deprecated Use getProductsByBrandAndCategory instead */
  getProductLegacy(brandId: number, categoryId: number): Observable<ProductByBrandCategory> {
    return this.getProductsByBrandAndCategory(brandId, categoryId);
  }
}
