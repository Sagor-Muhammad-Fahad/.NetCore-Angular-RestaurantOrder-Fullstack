import { Order } from './order';
import { OrderItem } from './order-item';

export interface MenuItem {
  menuItemId?: number;
  itemName: string;
  orderItems?: OrderItem[];
}
