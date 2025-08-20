import { Routes } from '@angular/router';
import { LoginComponent } from './Authorization/login.component';
import { RegisterComponent } from './Authorization/register.component';
import { ImageUploadComponent } from './Images/image-upload.component';
import { BrandListComponent } from './Brands/brand-list.component';
import { CategoryListComponent } from './Categories/category-list.component';
import { ImageListComponent } from './Images/image-list.component';
import { PublicImageListComponent } from './Images/public-image-list.component';
import { NotFoundComponent } from './shared/not-found.component';
import { AdminDashboardComponent } from './User/admin-dashboard.component';
import { AuthGuard } from './Authorization/auth.guard';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './Authorization/auth.service';
import { ProductManagementComponent } from './Product/product-management.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'upload-image', component: ImageUploadComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'brands', component: BrandListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'categories', component: CategoryListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'images', component: ImageListComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard], data: { role: 'Admin' } },
  { path: 'public-gallery', component: PublicImageListComponent },
  {
    path: 'admin/product',
    component: ProductManagementComponent,
    canActivate: [AuthGuard],
    data: { role: 'Admin' }
  },
  // Set the default route to public-gallery
  { path: '', redirectTo: 'public-gallery', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent },
];
