import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class ClothService {
private get baseUrl(): string {
  return environment.apiUrl;
}
  constructor(private http: HttpClient) { }

  getClothById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Cloth/${id}`);
  }

  getAllCloths(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Cloth`);
  }

  createCloth(cloth: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Cloth`, cloth);
  }

  updateCloth(cloth: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Cloth`, cloth);
  }

  deleteCloth(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Cloth/${id}`);
  }
}
