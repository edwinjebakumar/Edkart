import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IProduct } from '../shared/models/product';
import { IProductType } from '../shared/models/producttype';
import { ShopService } from './shop.service';
import { ShopParams } from '../shared/models/shopParams';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search', { static: false }) searchElement: ElementRef;
  products: IProduct[];
  brands: IBrand[];
  productTypes: IProductType[];
  shopParams: ShopParams;
  totalCount: number;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' },
  ];

  constructor(private shopService: ShopService) {
    this.shopParams = this.shopService.getShopParams();
  }

  ngOnInit(): void {
    this.getProducts(true);
    this.getBrands();
    this.getProductTypes();
  }

  getProducts(useCache = false) {
    this.shopService.getProducts(useCache).subscribe(response => {
      this.products = response.data;
      // this.shopParams.pageNumber = response.pageIndex;
      // this.shopParams.pageSize = response.pageSize;
      this.totalCount = response.count;
    },
      error => {
        console.log(error);
      })
  }

  getBrands() {
    this.shopService.getBrands().subscribe(response => {
      this.brands = [{ id: 0, name: 'All' }, ...response];
    },
      error => {
        console.log(error);
      })
  }

  getProductTypes() {
    this.shopService.getProductTypes().subscribe(response => {
      this.productTypes = [{ id: 0, name: 'All' }, ...response];
    },
      error => {
        console.log(error);
      })
  }

  onBrandSelected(brandId: number) {
    //const params = this.shopService.getShopParams();
    this.shopParams.brandId = brandId;
    this.shopParams.pageNumber = 1;
    //params.brandId = brandId;
    //params.pageNumber = 1;
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.shopParams.typeId = typeId;
    this.shopParams.pageNumber = 1;
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

  onSortSelected(sort: string) {
    this.shopParams.sort = sort;
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

  onPageChanged(event: any) {
    //alert(event);
    if (this.shopParams.pageNumber != event) {
      this.shopParams.pageNumber = event;
      this.shopService.setShopParams(this.shopParams);
      this.getProducts(true);
    }

  }

  onSearch() {
    this.shopParams.search = this.searchElement.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }

  onReset() {
    this.searchElement.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }
}
