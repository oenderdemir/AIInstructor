import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { AppFloatingConfigurator } from '../layout/component/app.floatingconfigurator';
import { AuthService } from './auth.service';
import { enumKullaniciStatus } from '../kullanici-yonetimi/kullanici-status.enum';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ButtonModule, CheckboxModule, InputTextModule, PasswordModule, FormsModule, RouterModule, RippleModule, AppFloatingConfigurator],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  email: string = '';

  password: string = '';

  checked: boolean = false;

  constructor(private router: Router, private authService:AuthService)
  {
    
  }
  login() {
    this.authService.login(this.email,this.password).subscribe(
      {
        next:response => {
          if (response) {
            //  this.layoutService.config.menuMode='static';
            
            if(response.kullaniciStatus==enumKullaniciStatus.SifreDegistirmeli)
            {
              localStorage.setItem("extoken",response.authToken);
              this.router.navigate(['/reset-password']);
              
            } 
            else
            {
              localStorage.setItem("token",response.authToken);
              this.router.navigate(['/']);
            }  
            
          }      
        },
        error:err=>{
          alert('Hatalı kullanıcı adı veya şifre');
        }
          
      }
  );
} 
   
}
