import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.css',
})
export class TestErrorsComponent {
  baseUrl = environment.apiUrl;
  http = inject(HttpClient);
  formValidationErrors: string[] = [];

  get400Error() {
    this.http.get(this.baseUrl + 'errorHandling/bad-request').subscribe({
      next: (response: any) => {
        console.log(response);
      },
      error: (err: any) => {
        console.log(err);
      },
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'errorHandling/auth').subscribe({
      next: (response: any) => console.log(response),
      error: (err: any) => {
        console.log(err, '\nYO', err.error.text);
      },
    });
  }

  get404Error() {
    this.http.get(this.baseUrl + 'errorHandling/not-found').subscribe({
      next: (response: any) => console.log(response),
      error: (err: any) => {
        console.log(err);
      },
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + 'errorHandling/server-error').subscribe({
      next: (response: any) => console.log(response),
      error: (err: any) => {
        console.log(err);
      },
    });
  }

  post400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: (response: any) => console.log(response),
      error: (err: any) => {
        this.formValidationErrors = err;
      },
    });
  }
}
