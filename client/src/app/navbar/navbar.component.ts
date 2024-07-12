import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [FormsModule, BsDropdownModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  accountService = inject(AccountService);
  username : any;
  model: any = {};
  title = 'Dating App 💖';

  login() {
    // console.log(this.model);
    this.accountService.loginSvc(this.model).subscribe({
      next: (resposne) => {
        console.log("Here", resposne);
      },
      error: (err) => console.log(`Error ocuured - ${err}\n`, err),
    });
  }

  logout(){
    this.accountService.logoutSvc();
  }
}
