import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastrSvc = inject(ToastrService);

  username : any;
  model: any = {};
  title = 'Dating App 💖';

  login() {
    // console.log(this.model);
    this.accountService.loginSvc(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.username = this.accountService.currentUser();
        console.log("Login Success");
      },
      error: (err) => {
        console.log(`Error ocuured - ${err}\n`, err);
        this.toastrSvc.error(err.error);
      },
    });
  }

  logout(){
    this.accountService.logoutSvc();
    this.router.navigateByUrl('/');
  }
}
