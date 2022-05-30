import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';
import { IBasket } from 'src/app/shared/models/basket';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
  @Input() appStepper: CdkStepper
  basket$: Observable<IBasket>;

  constructor(private basketService: BasketService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.basket$ = this.basketService.basket$;
  }

  createOrUpdatePaymentIntent() {
    this.basketService.createOrUpdatePaymentIntent().subscribe(
      (response: any) => {
        console.log(response);
        //this.toastrService.success('Payment Intent created successfully');
        this.appStepper.next();
      },
      error => {
        console.log(error);
        this.toastrService.error(error);
      }
    )
  }

}
