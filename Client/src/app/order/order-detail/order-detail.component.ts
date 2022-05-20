import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { IOrder } from 'src/app/shared/models/order';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrderService } from '../order.service';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss']
})
export class OrderDetailComponent implements OnInit {
  // order$: Observable<IOrder>;
  order: IOrder;

  constructor(private orderService: OrderService,
              private breadcrumbService: BreadcrumbService,
              private activatedRoute: ActivatedRoute
              ) { 
                this.breadcrumbService.set('@orderDetail','');
              }

  ngOnInit(): void {
    this.getUserOrderById(this.activatedRoute.snapshot.paramMap.get('id'));
    // this.order$ = this.orderService.order$;
    
  }

  getUserOrderById(id: string) {
    this.orderService.getUserOrderById(id).subscribe(
      (order: IOrder) => {
        console.log(order);
        this.order = order;
        this.breadcrumbService.set('@orderDetail', `Order# ${id} - ${order.status}`);
      },
      error => {
        console.log(error);
      }
    );
  }

}
