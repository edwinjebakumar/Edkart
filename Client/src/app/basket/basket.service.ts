import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Basket, IBasket, IBasketItem, IBasketTotals } from '../shared/models/basket';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiURL;

  private basketSource = new BehaviorSubject<IBasket>(null);
  basket$ = this.basketSource.asObservable();

  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);
  basketTotal$ = this.basketTotalSource.asObservable();

  shipping = 0;

  constructor(private httpClient: HttpClient) { }

  createOrUpdatePaymentIntent() {
    return this.httpClient.post(this.baseUrl + 'payments/' + this.getCurrentBasketValue().id, {}).pipe(
      map((basket: IBasket) => {
        this.basketSource.next(basket);
        console.log(this.getCurrentBasketValue());
      })
    );
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    var basket: IBasket = this.getCurrentBasketValue();
    basket.deliveryMethodId = deliveryMethod.id;
    basket.shippingPrice = deliveryMethod.price;
    this.setBasket(basket);
    //this.calculateTotals();
  }

  getBasket(id: string) {
    return this.httpClient.get(this.baseUrl + 'basket?id=' + id).pipe(
      map((basket: IBasket) => {
        this.basketSource.next(basket);
        this.shipping = basket.shippingPrice;
        //console.log(this.getCurrentBasketValue);
        this.calculateTotals();
      }))
  }

  addItemToBasket(product: IProduct, quantity = 1) {
    const basketItemToAdd: IBasketItem = this.createMappingProduct(product, quantity);
    var basket: IBasket = this.getCurrentBasketValue() ?? this.createBasket();
    //console.log(basket);
    basket.items = this.createOrUpdateBasket(basket.items, basketItemToAdd, quantity);
    this.setBasket(basket);
  }

  incrementItemQuantity(item: IBasketItem) {
    const currentBasket = this.getCurrentBasketValue();
    const foundItemIndex = currentBasket.items.findIndex(i => i.id === item.id);
    currentBasket.items[foundItemIndex].quantity++;
    this.setBasket(currentBasket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const currentBasket = this.getCurrentBasketValue();
    const foundItemIndex = currentBasket.items.findIndex(i => i.id === item.id);
    if (currentBasket.items[foundItemIndex].quantity > 1) {
      currentBasket.items[foundItemIndex].quantity--;
    }
    else {
      this.removeItemFromBasket(item);
    }

    this.setBasket(currentBasket);
  }

  removeItemFromBasket(item: IBasketItem) {
    const currentBasket = this.getCurrentBasketValue();
    if (currentBasket.items.some(i => i.id === item.id)) {
      currentBasket.items = currentBasket.items.filter(i => i.id != item.id);
      if (currentBasket.items.length > 0) {
        this.setBasket(currentBasket);
      }
      else {
        this.deleteBasket(currentBasket);
      }
    }
  }

  deleteLocalBasket(id: string) {
    this.basketSource.next(null);
    this.basketSource.next(null);
    localStorage.removeItem('basket_id');
  }

  deleteBasket(basket: IBasket) {
    return this.httpClient.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe(
      () => {
        this.basketSource.next(null);
        this.basketTotalSource.next(null);
        localStorage.removeItem('basket_id');
      },
      error => {
        console.log(error);
      });
  }

  private setBasket(basket: IBasket) {
    this.httpClient.post(this.baseUrl + 'basket/', basket).subscribe(
      (response: IBasket) => {
        this.basketSource.next(response);
        //console.log(response);
        this.calculateTotals();
      },
      error => {
        console.log(error);
      })
  }

  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  private createOrUpdateBasket(items: IBasketItem[], basketItemToAdd: IBasketItem, quantity: number) {
    console.log(items);
    const index = items.findIndex(i => i.id == basketItemToAdd.id);
    if (index == -1) {
      basketItemToAdd.quantity = quantity;
      items.push(basketItemToAdd);
    }
    else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private createBasket(): IBasket {
    const newBasket = new Basket();
    localStorage.setItem('basket_id', newBasket.id);
    return newBasket;
  }

  private createMappingProduct(product: IProduct, quantity: number): IBasketItem {
    return {
      id: product.id,
      productName: product.name,
      brand: product.productBrand,
      type: product.productType,
      pictureUrl: product.pictureUrl,
      price: product.price,
      quantity
    };
  }

  private calculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    const subTotal = basket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subTotal + shipping;
    this.basketTotalSource.next({ shipping, subTotal, total })
  }
}
