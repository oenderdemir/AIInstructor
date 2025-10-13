import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { AppFloatingConfigurator } from '../layout/component/app.floatingconfigurator';

import { enumKullaniciStatus } from '../kullanici-yonetimi/kullanici-status.enum';
import { AuthService } from '../authentication/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ButtonModule, CheckboxModule, InputTextModule, PasswordModule, FormsModule, RouterModule, RippleModule, AppFloatingConfigurator],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent {


  newPassword: string = '';
  currentPassword: string = '';
  newPassword2: string = '';



  constructor(private router: Router, private authService:AuthService)
  {
    
  }
  changePassword() {
    this.authService.changePassword(this.currentPassword,this.newPassword,this.newPassword2).subscribe({
      next:response => {
        if (response) {
          //  this.layoutService.config.menuMode='static';
          
          this.authService.logout();
          this.router.navigate(['/login']);
          
        }      
      },
      error:err=>{
        alert('Hatalı kullanıcı adı veya şifre');
      }
        
    });
  } 
   
}
