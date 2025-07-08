import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class BrandService {
private get baseUrl(): string {
  return environment.apiUrl;
}
  constructor(private http: HttpClient) { }

  getBrandById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Brand/${id}`);
  }

  getAllBrands(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Brand`);
  }

  createBrand(brand: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Brand`, brand);
  }

  updateBrand(brand: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Brand`, brand);
  }

  deleteBrand(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Brand/${id}`);
  }
}
