import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent implements OnInit {
  baseUrl = environment.apiURL;
  validationErrors: any;

  constructor(private httpClient: HttpClient) { }

  ngOnInit(): void {
  }

  get404Error() {
    this.httpClient.get(this.baseUrl + 'products/42').subscribe(
      response => {
        console.log(response);
      },
      error => {
        console.log(error);
      }
    );
  }

  get400Error() {
    this.httpClient.get(this.baseUrl + 'buggy/badrequest').subscribe(
      response => {
        console.log(response);
      },
      error => {
        console.log(error);
      }
    );
  }

  get400ValidationError() {
    this.httpClient.get(this.baseUrl + 'products/forty').subscribe(
      response => {
        console.log(response);
      },
      error => {
        console.log(error);
        this.validationErrors = error.errors; 
      }
    );
  }

  get500Error() {
    this.httpClient.get(this.baseUrl + 'Buggy/servererror').subscribe(
      response => {
        console.log(response);
      },
      error => {
        console.log(error);
      }
    );
  }

}
