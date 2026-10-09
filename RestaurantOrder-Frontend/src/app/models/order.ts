import { OrderItem } from './order-item';
import { RestaurantTable } from './restaurant-table';

export interface Order {
  orderId?: number;
  customerName: string;
  mobileNo: string;
  orderDate: string;
  receiptImageUrl?: string | null;
  isPaid: boolean;
  totalAmount: number;
  restaurantTableId: number;
  restaurantTable?: RestaurantTable;
  orderItems: OrderItem[];
}
