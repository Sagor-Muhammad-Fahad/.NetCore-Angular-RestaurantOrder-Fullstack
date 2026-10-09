import { Routes } from '@angular/router';

import { OrderComponents } from './components/order-components/order-components';

export const routes: Routes = [
  { path: 'order', component: OrderComponents },

  // { path: 'add-order', component: AddOrder },
  // { path: 'edit-order/:id', component: AddOrder },
  // { path: 'edit/:id', component: AddOrder },
  { path: '**', redirectTo: 'order' },
  { path: '', redirectTo: 'order', pathMatch: 'full' },
];
