import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
private get apiUrl(): string {
  return `${environment.apiUrl}/Category`;
}
private get brandUrl(): string {
  return `${environment.apiUrl}/Brand`;
}
  constructor(private http: HttpClient) { }

  getAllCategories(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCategoryById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getCategoriesByBrandId(brandId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/by-brand/${brandId}`);
  }

  createCategory(categoryData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, categoryData);
  }

  updateCategory(categoryData: any): Observable<any> {
    return this.http.put<any>(this.apiUrl, categoryData);
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
