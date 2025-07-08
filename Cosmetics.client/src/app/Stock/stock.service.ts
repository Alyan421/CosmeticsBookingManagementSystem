import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment'; // Make sure this path is correct!

export interface Stock {
  brandId: number;
  categoryId: number;
  brandName: string;
  categoryName: string;
  availableStock: number;
}

export interface StockUpdate {
  brandId: number;
  categoryId: number;
  availableStock: number;
}

@Injectable({
  providedIn: 'root'
})
export class StockService {
private get baseUrl(): string {
  return `${environment.apiUrl}/Stock`;
}
  constructor(private http: HttpClient) { }

  getAllStock(): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}`);
  }

  getStockByBrand(brandId: number): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}/brand/${brandId}`);
  }

  getStockByCategory(categoryId: number): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}/category/${categoryId}`);
  }

  getStock(brandId: number, categoryId: number): Observable<Stock> {
    return this.http.get<Stock>(`${this.baseUrl}/${brandId}/${categoryId}`);
  }

  updateStock(stock: StockUpdate): Observable<Stock> {
    return this.http.put<Stock>(`${this.baseUrl}`, stock);
  }

  bulkUpdateStock(stocks: StockUpdate[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bulk`, stocks);
  }

  deleteStock(brandId: number, categoryId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${brandId}/${categoryId}`);
  }
}
