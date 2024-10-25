import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../_services/loading.service';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { environment } from '../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  loadingService.loading();
  return next(req).pipe(
    environment.production ? identity : delay(700),
    finalize(() => {
      loadingService.idle();
    })
  );
};
