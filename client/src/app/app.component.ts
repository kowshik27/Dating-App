import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { AccountService } from './_services/account.service';
import { HomeComponent } from './home/home.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor, NavbarComponent, HomeComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {

  /* constructor(private httpClient:HttpClient) {};
        ---> This is old method (class based)*/
  http = inject(HttpClient); // This is new approach
  private accountService = inject(AccountService);
  title = 'Dating App 💖';
  users : any;
  displayUsers : any;

  // Interface
  ngOnInit(): void {
    this.getUsers();
    this.persistUser();
  };

  persistUser(): void{
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  getUsers(): void{
    this.http.get('http://localhost:5000/api/users').subscribe({
      next:(response)=>{
        this.users = response
        if(this.users!=null) this.displayUsers = this.users.filter((x: { id: number; })=> x.id<=5);
      },
      error: error =>console.log("Error is - ", error),
      complete: ()=> console.log(`Fetching completed`)
    })
  }
      
  
}
