import { Routes } from '@angular/router';
import { LoginComponent } from './Authorization/login.component';
import { RegisterComponent } from './Authorization/register.component';
import { ImageUploadComponent } from './Images/image-upload.component';
import { ClothListComponent } from './Cloths/cloth-list.component';
import { ColorListComponent } from './Colors/color-list.component';
import { ImageListComponent } from './Images/image-list.component';
import { PublicImageListComponent } from './Images/public-image-list.component';
import { NotFoundComponent } from './shared/not-found.component';
import { AdminDashboardComponent } from './User/admin-dashboard.component';
import { AuthGuard } from './Authorization/auth.guard';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './Authorization/auth.service';
import { StockManagementComponent } from './Stock/stock-management.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'upload-image', component: ImageUploadComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'cloths', component: ClothListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'colors', component: ColorListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'images', component: ImageListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'public-gallery', component: PublicImageListComponent },
  {
    path: 'admin/stock',
    component: StockManagementComponent,
    canActivate: [AuthGuard],
    data: { role: 'Admin' }
  },
  {
    path: '',
    canActivate: [AuthGuard],
    resolve: {
      role: (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
        const authService = inject(AuthService);
        const router = inject(Router);
        const role = authService.getCurrentUserRole();

        if (role === 'Admin') {
          router.navigate(['/admin-dashboard']);
        } else {
          router.navigate(['/public-gallery']);
        }
        return role;
      }
    },
    component: NotFoundComponent
  },
  { path: '**', component: NotFoundComponent },
];
