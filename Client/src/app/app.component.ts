import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'EdKart';
  // products:IProduct[]

  constructor(private basketService: BasketService,
    private accountService: AccountService) { }

  ngOnInit(): void {
    // this.httpClient.get("https://localhost:5001/api/Products?pagesize=50").subscribe(
    //   (response: IPagination) => {
    //     console.log(response);
    //     this.products = response.data;
    //   },
    //   error => {
    //     console.log(error);
    //   })

    this.loadCurrentUser();
    this.loadBasket();
  }

  loadCurrentUser() {
    const token = localStorage.getItem('token');
      this.accountService.loadCurrentUser(token).subscribe(
        () => {
          console.log('Loaded User');
        },
        error => {
          console.log(error);
        }
      )
  }

  loadBasket() {
    const basketId = localStorage.getItem('basket_id');
    if (basketId) {
      this.basketService.getBasket(basketId).subscribe(
        () => {
          console.log("Basket initialized");
        },
        error => {
          console.log(error);
        });
    }
  }
}
