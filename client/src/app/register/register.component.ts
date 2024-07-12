import { Component, inject, output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { FormsModule} from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  cancelRegister = output<boolean>();

  model:any={};
  private accountService = inject(AccountService);

  register(){
    this.accountService.registerSvc(this.model).subscribe({
      next: (response)=>{
        console.log("Reg Comp res =>", response);
        this.cancel();
      },
      error: err => console.log(`Error occured is`, err),
    });
  }

  cancel(){
    console.log("Registration Cancelled !!");
    this.cancelRegister.emit(false);
  }
}
