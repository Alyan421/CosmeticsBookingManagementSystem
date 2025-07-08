import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
private get baseUrl(): string {
  return environment.apiUrl};
  constructor(private http: HttpClient) { }

  getAllImages(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image`);
  }

  getImagesByCategoryId(categoryId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/by-category/${categoryId}`);
  }

  uploadImage(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Image/upload/`, formData);
  }

  updateImage(id: number, formData: FormData): Observable<any> {
    // Ensure we're using the right HTTP method and URL format
    return this.http.put<any>(`${this.baseUrl}/Image/${id}`, formData);
  }

  deleteImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Image/${id}`);
  }

  filterByCategoryName(categoryName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/filter-by-category/${categoryName}`);
  }

  filterByBrandName(brandName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/filter-by-brand-name/${brandName}`);
  }
}
