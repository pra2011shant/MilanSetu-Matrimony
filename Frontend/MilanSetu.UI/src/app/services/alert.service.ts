import { Injectable } from '@angular/core';
import Swal, { SweetAlertOptions, SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  
  // Crimson & Gold custom styled toast
  private Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3500,
    timerProgressBar: true,
    didOpen: (toast) => {
      toast.onmouseenter = Swal.stopTimer;
      toast.onmouseleave = Swal.resumeTimer;
    }
  });

  // Success Toast
  toastSuccess(message: string, title: string = 'Success') {
    this.Toast.fire({
      icon: 'success',
      title: title ? `<strong>${title}</strong><br><small>${message}</small>` : message,
      iconColor: '#e11d48'
    });
  }

  // Error Toast
  toastError(message: string, title: string = 'Error') {
    this.Toast.fire({
      icon: 'error',
      title: title ? `<strong>${title}</strong><br><small>${message}</small>` : message,
      iconColor: '#ef4444'
    });
  }

  // Info Toast
  toastInfo(message: string, title: string = 'Information') {
    this.Toast.fire({
      icon: 'info',
      title: title ? `<strong>${title}</strong><br><small>${message}</small>` : message,
      iconColor: '#3b82f6'
    });
  }

  // Success Modal
  success(title: string, text: string): Promise<SweetAlertResult> {
    return Swal.fire({
      title: title,
      text: text,
      icon: 'success',
      confirmButtonText: 'Great!',
      confirmButtonColor: '#e11d48',
      background: '#ffffff',
      customClass: {
        popup: 'milansetu-swal-popup',
        confirmButton: 'milansetu-swal-btn'
      }
    });
  }

  // Error Modal
  error(title: string, text: string): Promise<SweetAlertResult> {
    return Swal.fire({
      title: title,
      text: text,
      icon: 'error',
      confirmButtonText: 'Okay',
      confirmButtonColor: '#e11d48',
      customClass: {
        popup: 'milansetu-swal-popup'
      }
    });
  }

  // Confirmation Dialog
  confirm(title: string, text: string, confirmBtnText: string = 'Yes, Proceed'): Promise<SweetAlertResult> {
    return Swal.fire({
      title: title,
      text: text,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#e11d48',
      cancelButtonColor: '#64748b',
      confirmButtonText: confirmBtnText,
      cancelButtonText: 'Cancel',
      reverseButtons: true,
      customClass: {
        popup: 'milansetu-swal-popup'
      }
    });
  }
}
