import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from './auth/jwt.interceptor';
import { errorInterceptor } from './interceptors/error.interceptor';



@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  providers: [
    provideHttpClient(
      withInterceptors([jwtInterceptor, errorInterceptor])
    )
  
  ]
})
export class CoreModule { 
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    // Prevent reimporting the CoreModule
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in the AppModule only');
    }
  }
}
