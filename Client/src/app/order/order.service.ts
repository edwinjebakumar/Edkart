import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IOrder } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiURL;

  // private ordersSource = new BehaviorSubject<IOrder[]>(null);
  // orders$ = this.ordersSource.asObservable();

  // private orderSource = new BehaviorSubject<IOrder>(null);
  // order$ = this.orderSource.asObservable();

  constructor(private httpClient: HttpClient) { }

  getUserOrders() {
    // return this.httpClient.get(this.baseUrl + 'orders').pipe(
    //   map((orders: IOrder[]) => {
    //     this.ordersSource.next(orders);
    //   })
    // );
    return this.httpClient.get(this.baseUrl + 'orders');
  }


  getUserOrderById(id: string) {
    // return this.httpClient.get<IOrder>(this.baseUrl + 'orders/' + id).pipe(
    //   map((order: IOrder) => {
    //     this.orderSource.next(order);
    //     return order;
    //   })
    // );
    return this.httpClient.get<IOrder>(this.baseUrl + 'orders/' + id);
  }
}
