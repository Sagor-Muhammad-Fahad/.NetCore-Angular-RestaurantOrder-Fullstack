import { MenuItem } from './menu-item';
import { Order } from './order';

export interface OrderItem {
  orderItemId?: number;
  quantity: number;
  orderId?: number;
  order?: Order;
  menuItemId: number;
  menuItem?: MenuItem;
}
