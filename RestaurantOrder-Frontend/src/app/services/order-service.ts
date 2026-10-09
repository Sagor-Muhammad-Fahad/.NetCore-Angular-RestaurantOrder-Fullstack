import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../models/order';
import { UploadResponse } from '../models/upload-response';
import { RestaurantTable } from '../models/restaurant-table';
import { MenuItem } from '../models/menu-item';

const apiBaseUrl = 'http://localhost:5095/api/Orders';

@Injectable({ providedIn: 'root' })
export class OrderService {
  // getMenuItems() {
  //   throw new Error('Method not implemented.');
  // }

  private http = inject(HttpClient);

  getOrdersWithDetails(): Observable<Order[]> {
    return this.http.get<Order[]>(`${apiBaseUrl}/Item/Include`);
  }

  getOrderByIdWithDetails(id: number): Observable<Order> {
    return this.http.get<Order>(`${apiBaseUrl}/${id}/Include`);
  }

  saveOrder(order: Order): Observable<Order> {
    return this.http.post<Order>(apiBaseUrl, order);
  }

  uploadImage(id: number, file: File): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UploadResponse>(`${apiBaseUrl}/Upload/${id}`, formData);
  }

  updateOrder(id: number, order: Order): Observable<void> {
    return this.http.put<void>(`${apiBaseUrl}/${id}`, order);
  }

  deleteOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${apiBaseUrl}/${id}`);
  }

  getRestaurantTables(): Observable<RestaurantTable[]> {
    return this.http.get<RestaurantTable[]>(`${apiBaseUrl}/restaurantTables`);
  }

  getMenuItem(): Observable<MenuItem[]> {
    return this.http.get<MenuItem[]>(`${apiBaseUrl}/menuItems`);
  }
}
