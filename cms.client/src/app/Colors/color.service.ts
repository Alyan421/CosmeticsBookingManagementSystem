import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ColorService {
private get apiUrl(): string {
  return `${environment.apiUrl}/Color`;
}
private get clothUrl(): string {
  return `${environment.apiUrl}/Cloth`;
}
  constructor(private http: HttpClient) { }

  getAllColors(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getColorById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getColorsByClothId(clothId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/by-cloth/${clothId}`);
  }

  createColor(colorData: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, colorData);
  }

  updateColor(colorData: any): Observable<any> {
    return this.http.put<any>(this.apiUrl, colorData);
  }

  deleteColor(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
