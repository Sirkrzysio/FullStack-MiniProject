import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl = 'http://localhost:5187/api/Files';

  constructor(private http: HttpClient) {}

  uploadFile(file: File): Observable<{ count: number }> {
    const formData = new FormData();
    formData.append('file', file);

    // Pobieramy token JWT z localStorage
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    // POST do backendu z plikiem i nagłówkiem autoryzacji
    return this.http.post<{ count: number }>(`${this.apiUrl}/upload`, formData, { headers });
  }
}
