import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment'; // Make sure this path is correct!

export interface Product {
  brandId: number;
  categoryId: number;
  brandName: string;
  categoryName: string;
  availableProduct: number;
}

export interface ProductUpdate {
  brandId: number;
  categoryId: number;
  availableProduct: number;
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

  getProductByBrand(brandId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/brand/${brandId}`);
  }

  getProductByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/category/${categoryId}`);
  }

  getProduct(brandId: number, categoryId: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${brandId}/${categoryId}`);
  }

  updateProduct(product: ProductUpdate): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}`, product);
  }

  bulkUpdateProduct(products: ProductUpdate[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bulk`, products);
  }

  deleteProduct(brandId: number, categoryId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${brandId}/${categoryId}`);
  }
}
