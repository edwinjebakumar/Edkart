import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IOrder } from '../shared/models/order';
import { OrderService } from './order.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})
export class OrderComponent implements OnInit {
  orders: IOrder[];
  // orders$: Observable<IOrder[]>;

  constructor(private orderService: OrderService) { }

  ngOnInit(): void {
    this.getUserOrders();

  }

  getUserOrders() {
    this.orderService.getUserOrders().subscribe(
      (orders: IOrder[]) => {
        console.log(orders);
        this.orders = orders;
      },
      error => {
        console.log(error);
      }
    );
  }

  // getUserOrderById(id: string) {
  //   this.orderService.getUserOrderById(id);
  // }

}
