import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'EdKart';
  // products:IProduct[]

  constructor() { }

  ngOnInit(): void {
    // this.httpClient.get("https://localhost:5001/api/Products?pagesize=50").subscribe(
    //   (response: IPagination) => {
    //     console.log(response);
    //     this.products = response.data;
    //   },
    //   error => {
    //     console.log(error);
    //   })
  }
}
