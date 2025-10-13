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
import { UlkeYonetimiComponent } from './app/ulke-yonetimi/ulke-yonetimi.component';
import { MusteriYonetimiComponent } from './app/musteri-yonetimi/musteri-yonetimi.component';
import { ApnYonetimiComponent } from './app/apn-yonetimi/apn-yonetimi.component';
import { MarkaYonetimiComponent } from './app/marka-yonetimi/marka-yonetimi.component';
import { CihazSeriYonetimiComponent } from './app/cihaz-seri-yonetimi/cihaz-seri-yonetimi.component';
import { TcuModelYonetimiComponent } from './app/tcu-model-yonetimi/tcu-model-yonetimi.component';
import { DistrubitorYonetimiComponent } from './app/distributor-yonetimi/distributor-yonetimi.component';
import { ManufacturerYonetimiComponent } from './app/manufacturer-yonetimi/manufacturer-yonetimi.component';
import { CihazYonetimiComponent } from './app/cihaz-yonetimi/cihaz-yonetimi.component';

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
            { path: 'ulkeler', component: UlkeYonetimiComponent }, 
            { path: 'musteriler', component: MusteriYonetimiComponent }, 
            { path: 'apnler', component: ApnYonetimiComponent },
            { path: 'markalar', component: MarkaYonetimiComponent }, 
            { path: 'tcu-modeller', component: TcuModelYonetimiComponent }, 
            { path: 'distributorlar', component: DistrubitorYonetimiComponent }, 
            { path: 'ureticiler', component: ManufacturerYonetimiComponent }, 
            { path: 'cihaz-serileri', component: CihazSeriYonetimiComponent }, 
            { path: 'testler', component: CihazYonetimiComponent }, 
           
        ], canActivate: [AuthGuard] 
    },
    { path: 'login', component: LoginComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: '**', redirectTo: '/not-found' } // Burada 404 sayfasına yönlendiriyoruz
    
];
