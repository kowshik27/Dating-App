import {
  Directive,
  inject,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]', // *appHasRole is used
  standalone: true,
})
export class HasRoleDirective implements OnInit {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  @Input() appHasRole: string[] = [];
  // appHasRole = input.required<string[]>();

  ngOnInit(): void {
    if (
      this.accountService
        .roles()
        ?.some((r: string) => this.appHasRole.includes(r))
    ) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
