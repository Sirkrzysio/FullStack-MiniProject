import {Routes} from '@angular/router';
import {Home} from './components/pages/home/home';
import {About} from './components/pages/about/about';
import {Contact} from './components/pages/contact/contact';
import {LoginComponent} from './components/auth/login/login';
import {RegisterComponent} from './components/auth/register/register';


export const routes: Routes = [
  { path: '', component: Home },
  { path: 'about', component: About },
  { path: 'contact', component: Contact },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
];


