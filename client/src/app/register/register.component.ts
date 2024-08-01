import { Component, inject, OnInit, output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { NgIf } from '@angular/common';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DateInputComponent } from '../_forms/date-input/date-input.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, TextInputComponent, DateInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  cancelRegister = output<boolean>();
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: [] = [];

  ngOnInit(): void {
    this.initializeRForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeRForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      nickName: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(10),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(10),
        ],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.passwordMatchValidator('password')],
      ],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      },
    });
  }

  passwordMatchValidator(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { mismatch: true };
    };
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({ dateOfBirth: dob });
    this.accountService.registerSvc(this.registerForm.value).subscribe({
      next: (_) => {
        this.router.navigateByUrl('members');
      },
      error: (err) => {
        console.log(err);
        this.validationErrors = err;
      },
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | null) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0, 10);
  }
}
