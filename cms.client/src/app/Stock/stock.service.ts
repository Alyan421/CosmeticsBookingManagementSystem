import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment'; // Make sure this path is correct!

export interface Stock {
  clothId: number;
  colorId: number;
  clothName: string;
  colorName: string;
  availableStock: number;
}

export interface StockUpdate {
  clothId: number;
  colorId: number;
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

  getStockByCloth(clothId: number): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}/cloth/${clothId}`);
  }

  getStockByColor(colorId: number): Observable<Stock[]> {
    return this.http.get<Stock[]>(`${this.baseUrl}/color/${colorId}`);
  }

  getStock(clothId: number, colorId: number): Observable<Stock> {
    return this.http.get<Stock>(`${this.baseUrl}/${clothId}/${colorId}`);
  }

  updateStock(stock: StockUpdate): Observable<Stock> {
    return this.http.put<Stock>(`${this.baseUrl}`, stock);
  }

  bulkUpdateStock(stocks: StockUpdate[]): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/bulk`, stocks);
  }

  deleteStock(clothId: number, colorId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${clothId}/${colorId}`);
  }
}
