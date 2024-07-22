import { inject, Injectable } from '@angular/core';
import { NgxSpinner, NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  loadingCounter = 0;
  private spinner = inject(NgxSpinnerService);

  loading() {
    this.loadingCounter++;
    this.spinner.show(undefined, {
      type: 'ball-scale-pulse',
      bdColor: 'rgba(255,255,255,0.8)',
      color: '#5696fc',
    });
  }

  idle() {
    this.loadingCounter--;
    if (this.loadingCounter <= 0) {
      this.loadingCounter = 0;
      this.spinner.hide();
    }
  }
}
