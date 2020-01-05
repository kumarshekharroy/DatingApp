import * as alerify from 'alertifyjs';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AlerifyServices {
  constructor() {}

  confirm(message: string, okCallback: () => any) {
    alerify.confirm(message, (e: any) => {
      if (e) {
        okCallback();
      } else {
      }
    });
  }
  error(message: string) {
    alerify.error(message);
  }
  success(message: string) {
    alerify.success(message);
  }
  warning(message: string) {
    alerify.warning(message);
  }
  message(message: string) {
    alerify.message(message);
  }
}
