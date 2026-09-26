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

  // Interactive Google Account Selection Dialog
  async promptGoogleAuth(actionText: string = 'Sign in with Google'): Promise<{ email: string; name: string; photoUrl: string } | null> {
    const { value: formValues } = await Swal.fire({
      title: `<div style="display:flex;align-items:center;justify-content:center;gap:10px;"><svg width="28" height="28" viewBox="0 0 24 24"><path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/><path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/><path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.06H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.94l2.85-2.22.81-.63z"/><path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.06l3.66 2.84c.87-2.6 3.3-4.52 6.16-4.52z"/></svg><span style="font-size:1.2rem;font-weight:700;color:#1e293b;">${actionText}</span></div>`,
      html: `
        <p style="font-size:0.875rem;color:#64748b;margin-bottom:16px;">Choose your Google account to connect to <strong>MilanSetu Matrimony</strong></p>
        <div style="text-align:left;margin-bottom:12px;">
          <label style="font-size:0.8rem;font-weight:600;color:#334155;display:block;margin-bottom:4px;">Google / Gmail Address *</label>
          <input id="swal-google-email" class="swal2-input" type="email" placeholder="your.name@gmail.com" style="margin:0;width:100%;box-sizing:border-box;font-size:0.95rem;height:42px;">
        </div>
        <div style="text-align:left;">
          <label style="font-size:0.8rem;font-weight:600;color:#334155;display:block;margin-bottom:4px;">Full Name *</label>
          <input id="swal-google-name" class="swal2-input" type="text" placeholder="e.g. Prashant Kumar" style="margin:0;width:100%;box-sizing:border-box;font-size:0.95rem;height:42px;">
        </div>
      `,
      focusConfirm: false,
      showCancelButton: true,
      confirmButtonText: '<i class="bi bi-google"></i> Continue with Google',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#1a73e8',
      cancelButtonColor: '#94a3b8',
      customClass: {
        popup: 'milansetu-swal-popup'
      },
      preConfirm: () => {
        const email = (document.getElementById('swal-google-email') as HTMLInputElement)?.value;
        const name = (document.getElementById('swal-google-name') as HTMLInputElement)?.value;
        if (!email || !email.includes('@')) {
          Swal.showValidationMessage('Please enter a valid Gmail / Google email address.');
          return false;
        }
        if (!name || name.trim().length < 2) {
          Swal.showValidationMessage('Please enter your full name.');
          return false;
        }
        return {
          email: email.trim().toLowerCase(),
          name: name.trim(),
          photoUrl: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&w=500&q=80'
        };
      }
    });

    return formValues || null;
  }
}
