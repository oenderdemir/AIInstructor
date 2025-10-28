import { Routes } from '@angular/router';
import { AppLayout } from './app/layout/component/app.layout';

import { LoginComponent } from './app/authentication/login.component';
import { AuthGuard } from './app/authentication/auth.guard';
import { NotFoundComponent } from './app/not-found/not-found.component';
import { MenuYonetimiComponent } from './app/menu-yonetimi/menu-yonetimi.component';
import { KullaniciYonetimiComponent } from './app/kullanici-yonetimi/kullanici-yonetimi.component';
import { KullaniciGrupYonetimiComponent } from './app/kullanici-grup-yonetimi/kullanici-grup-yonetimi.component';
import { RolYonetimiComponent } from './app/rol-yonetimi/rol-yonetimi.component';
import { ResetPasswordComponent } from './app/reset-password/reset-password.component';

import { SenaryoYonetimiComponent } from './app/senaryo-yonetimi/senaryo-yonetimi.component';
import { SenaryoSecimiComponent } from './app/senaryo-secimi/senaryo-secimi.component';
import { SenaryoCalistirComponent } from './app/senaryo-calistir/senaryo-calistir.component';

export const appRoutes: Routes = [
    {
        path: '', 
        component: AppLayout,
        children: [
            { path: 'not-found', component: NotFoundComponent },
            { path: 'menuler', component: MenuYonetimiComponent },
            { path: 'kullanicilar', component: KullaniciYonetimiComponent },
            { path: 'kullanici-gruplar', component: KullaniciGrupYonetimiComponent },
            { path: 'roller', component: RolYonetimiComponent },
            { path: 'demo-senaryo', component: SenaryoYonetimiComponent, data: { roles: ['DersYetkilisi'] } },
            { path: 'senaryo-secimi', component: SenaryoSecimiComponent, data: { roles: ['Ogrenci', 'DersYetkilisi'] } },
            { path: 'senaryo-calistir/:id', component: SenaryoCalistirComponent, data: { roles: ['Ogrenci', 'DersYetkilisi'] } },

        ], canActivate: [AuthGuard], canActivateChild: [AuthGuard]
    },
   
    { path: 'login', component: LoginComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: '**', redirectTo: '/not-found' } // Burada 404 sayfasına yönlendiriyoruz
    
];
