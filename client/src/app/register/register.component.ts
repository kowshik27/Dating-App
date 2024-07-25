import { Component, inject, output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private accountService = inject(AccountService);
  private toastr = inject(ToastrService);

  cancelRegister = output<boolean>();
  registerModel: any = {};

  register() {
    this.accountService.registerSvc(this.registerModel).subscribe({
      next: (response) => {
        console.log('Reg Comp res =>', response);
        this.cancel();
      },
      error: (err) => {
        console.log(`Error occured is`, err, Array.isArray(err));
        if (Array.isArray(err)) {
          for (let e of err) {
            this.toastr.error(e);
          }
        }
        else{
          this.toastr.error(err.error);
        }
      },
    });
  }

  cancel() {
    console.log('Registration Cancelled !!');
    this.cancelRegister.emit(false);
  }
}
