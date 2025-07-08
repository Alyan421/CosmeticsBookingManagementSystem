import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app/app-routing.module';
import { authInterceptor } from './app/Authorization/auth.interceptor';
// Remove Toastr imports

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations()
    // Remove importProvidersFrom(ToastrModule.forRoot())
  ]
}).catch(err => console.error(err));
