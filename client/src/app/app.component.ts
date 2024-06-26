import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {

  /* constructor(private httpClient:HttpClient) {};
        ---> This is old method (class based)*/
  http = inject(HttpClient); // This is new approach
  title = 'Dating App 💖';
  users : any;

  // Interface
  ngOnInit(): void {
    this.http.get('http://localhost:5000/api/users').subscribe({
      next:(response)=>{this.users = response},
      error: error =>console.log("Error is - ", error),
      complete: ()=> console.log(`Fetching completed`)
    })
  };
}
